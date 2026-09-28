const request = require('supertest');
const { io } = require('socket.io-client');
const { levantarEntorno, metadatosDe, grpc, XALAPA, CERCA_DE_XALAPA } = require('./entornoPruebas');

const MASCOTA = Object.freeze({ Nombre: 'Nala', Especie: 'Gato', Raza: 'Siamés', Edad: '1 año', Sexo: 'Hembra', Tamaño: 'Chico' });

function subirArchivo(metodo, token, { idReferencia, nombreArchivo, partes }) {
  return new Promise((resolver, rechazar) => {
    const flujo = metodo(metadatosDe(token), (error, respuesta) => (error ? rechazar(error) : resolver(respuesta)));
    flujo.write({ metadatos: { idReferencia, nombreArchivo } });
    partes.forEach((parte) => flujo.write({ datos: Buffer.from(parte) }));
    flujo.end();
  });
}

function conectarSocket(puerto, token) {
  return new Promise((resolver, rechazar) => {
    const socket = io(`http://127.0.0.1:${puerto}`, { auth: { token }, transports: ['websocket'], reconnection: false });
    socket.once('connect', () => resolver(socket));
    socket.once('connect_error', rechazar);
  });
}

describe('multimedia, notificaciones y chat en tiempo real', () => {
  let entorno;
  let ana;
  let luis;
  let adopcion;

  beforeAll(async () => {
    entorno = await levantarEntorno();
    ana = await entorno.registrarYEntrar({ nombre: 'Ana', correo: 'ana@correo.com', ubicacion: XALAPA });
    luis = await entorno.registrarYEntrar({ nombre: 'Luis', correo: 'luis@correo.com', ubicacion: CERCA_DE_XALAPA });
  });

  afterAll(() => entorno.cerrar());

  const como = (sesion) => `Bearer ${sesion.token}`;

  test('entrega por gRPC las notificaciones al suscriptor conectado', async () => {
    const flujo = entorno.clientesGrpc.notificacion.EscucharNotificaciones({}, metadatosDe(luis.token));
    const recibida = new Promise((resolver) => flujo.once('data', resolver));
    await new Promise((resolver) => setTimeout(resolver, 300));

    const { body } = await request(entorno.app).post('/api/adopciones').set('Authorization', como(ana))
      .send({ Mascota: MASCOTA, Ubicacion: XALAPA }).expect(201);
    adopcion = body;

    await expect(recibida).resolves.toMatchObject({ tipo: 'AdopcionCercana', referenciaId: adopcion.AdopcionID, referenciaTipo: 'Adopcion' });
    flujo.on('error', () => {});
    flujo.cancel();
  });

  test('sube y descarga la foto de una mascota propia', async () => {
    const multimedia = entorno.clientesGrpc.multimedia;
    await expect(subirArchivo(multimedia.SubirFotoMascota.bind(multimedia), ana.token, {
      idReferencia: adopcion.MascotaID, nombreArchivo: 'nala.png', partes: ['imagen-', 'de-prueba']
    })).resolves.toMatchObject({ exito: true });

    const foto = await request(entorno.app).get(`/api/mascotas/${adopcion.MascotaID}/foto`).set('Authorization', como(luis))
      .buffer(true).parse((respuesta, listo) => {
        const partes = [];
        respuesta.on('data', (parte) => partes.push(parte));
        respuesta.on('end', () => listo(null, Buffer.concat(partes)));
      })
      .expect(200);
    expect(foto.body.toString()).toBe('imagen-de-prueba');
  });

  test('impide subir archivos a mascotas ajenas y valida el formato', async () => {
    const multimedia = entorno.clientesGrpc.multimedia;
    await expect(subirArchivo(multimedia.SubirFotoMascota.bind(multimedia), luis.token, {
      idReferencia: adopcion.MascotaID, nombreArchivo: 'intruso.png', partes: ['x']
    })).rejects.toMatchObject({ code: grpc.status.PERMISSION_DENIED });

    await expect(subirArchivo(multimedia.SubirVideoMascota.bind(multimedia), ana.token, {
      idReferencia: adopcion.MascotaID, nombreArchivo: 'video.avi', partes: ['x']
    })).rejects.toMatchObject({ code: grpc.status.INVALID_ARGUMENT });
  });

  test('transmite el video por rangos', async () => {
    const multimedia = entorno.clientesGrpc.multimedia;
    await subirArchivo(multimedia.SubirVideoMascota.bind(multimedia), ana.token, {
      idReferencia: adopcion.MascotaID, nombreArchivo: 'nala.mp4', partes: ['0123456789']
    });

    const parcial = await request(entorno.app).get(`/api/mascotas/${adopcion.MascotaID}/video`)
      .set('Authorization', como(luis)).set('Range', 'bytes=2-5').expect(206);
    expect(parcial.headers['content-range']).toBe('bytes 2-5/10');
    expect(parcial.headers['content-length']).toBe('4');
  });

  test('sube la foto de perfil del usuario autenticado', async () => {
    const multimedia = entorno.clientesGrpc.multimedia;
    await subirArchivo(multimedia.SubirFotoUsuario.bind(multimedia), luis.token, {
      idReferencia: 0, nombreArchivo: 'perfil.jpg', partes: ['perfil']
    });
    await request(entorno.app).get('/api/usuarios/yo/foto').set('Authorization', como(luis)).expect(200);
    await request(entorno.app).get('/api/usuarios/yo/foto').set('Authorization', como(ana)).expect(404);
  });

  test('entrega los mensajes de chat en tiempo real y los conserva en el historial', async () => {
    const socketAna = await conectarSocket(entorno.puertos.puertoHttp, ana.token);
    const socketLuis = await conectarSocket(entorno.puertos.puertoHttp, luis.token);
    try {
      const recibidoPorAna = new Promise((resolver) => socketAna.once('nuevo_mensaje', resolver));
      const confirmadoALuis = new Promise((resolver) => socketLuis.once('nuevo_mensaje', resolver));
      socketLuis.emit('enviar_mensaje', { DestinatarioID: ana.usuario.UsuarioID, Contenido: 'Hola, me interesa Nala' });

      const mensaje = await recibidoPorAna;
      expect(mensaje).toMatchObject({ RemitenteID: luis.usuario.UsuarioID, Contenido: 'Hola, me interesa Nala' });
      await expect(confirmadoALuis).resolves.toMatchObject({ ChatID: mensaje.ChatID });
    } finally {
      socketAna.close();
      socketLuis.close();
    }

    const conversaciones = await request(entorno.app).get('/api/chat/conversaciones').set('Authorization', como(ana)).expect(200);
    expect(conversaciones.body).toEqual([expect.objectContaining({ UsuarioID: luis.usuario.UsuarioID, Nombre: 'Luis' })]);

    const historial = await request(entorno.app).get(`/api/chat/conversaciones/${luis.usuario.UsuarioID}/mensajes`)
      .set('Authorization', como(ana)).expect(200);
    expect(historial.body).toHaveLength(1);
  });

  test('rechaza conexiones de socket sin token', async () => {
    await expect(conectarSocket(entorno.puertos.puertoHttp, '')).rejects.toBeInstanceOf(Error);
  });
});
