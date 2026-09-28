const request = require('supertest');
const { levantarEntorno, metadatosDe, grpc, XALAPA, CERCA_DE_XALAPA } = require('./entornoPruebas');

const MASCOTA = Object.freeze({
  Nombre: 'Max', Especie: 'Perro', Raza: 'Mestizo', Edad: '2 año(s) con 3 mes(es)', Sexo: 'Macho', Tamaño: 'Mediano', Descripcion: 'Juguetón'
});

function obtenerCercanas(cliente, token, ubicacion) {
  return new Promise((resolver, rechazar) => {
    cliente.ObtenerAdopcionesCercanas(
      { latitud: ubicacion.Latitud, longitud: ubicacion.Longitud },
      metadatosDe(token),
      (error, respuesta) => (error ? rechazar(error) : resolver(respuesta.resultados))
    );
  });
}

describe('ciclo de vida de una adopción', () => {
  let entorno;
  let publicador;
  let interesado;
  let adopcion;

  beforeAll(async () => {
    entorno = await levantarEntorno();
    publicador = await entorno.registrarYEntrar({ nombre: 'Ana', correo: 'ana@correo.com', ubicacion: XALAPA });
    interesado = await entorno.registrarYEntrar({ nombre: 'Luis', correo: 'luis@correo.com', ubicacion: CERCA_DE_XALAPA });
  });

  afterAll(() => entorno.cerrar());

  const como = (sesion) => `Bearer ${sesion.token}`;

  test('publica una adopción y notifica al usuario cercano', async () => {
    const { body } = await request(entorno.app).post('/api/adopciones').set('Authorization', como(publicador))
      .send({ Mascota: MASCOTA, Ubicacion: XALAPA }).expect(201);
    adopcion = body;

    const notificaciones = await request(entorno.app).get('/api/notificaciones').set('Authorization', como(interesado)).expect(200);
    expect(notificaciones.body).toEqual([expect.objectContaining({ Tipo: 'AdopcionCercana', ReferenciaID: adopcion.AdopcionID })]);

    const propias = await request(entorno.app).get('/api/notificaciones').set('Authorization', como(publicador)).expect(200);
    expect(propias.body).toHaveLength(0);
  });

  test('la adopción aparece por gRPC para otros usuarios pero no para el publicador', async () => {
    const paraInteresado = await obtenerCercanas(entorno.clientesGrpc.ubicacion, interesado.token, CERCA_DE_XALAPA);
    expect(paraInteresado).toEqual([expect.objectContaining({
      adopcionId: adopcion.AdopcionID,
      publicadorId: publicador.usuario.UsuarioID,
      mascota: expect.objectContaining({ nombre: 'Max', tamano: 'Mediano' })
    })]);

    await expect(obtenerCercanas(entorno.clientesGrpc.ubicacion, publicador.token, XALAPA)).resolves.toEqual([]);
    await expect(obtenerCercanas(entorno.clientesGrpc.ubicacion, 'token-invalido', XALAPA))
      .rejects.toMatchObject({ code: grpc.status.UNAUTHENTICATED });
  });

  test('solo el publicador puede modificar la adopción', async () => {
    const ruta = `/api/adopciones/${adopcion.AdopcionID}`;
    await request(entorno.app).patch(ruta).set('Authorization', como(interesado)).send({ Mascota: { Nombre: 'Robo' } }).expect(403);
    const { body } = await request(entorno.app).patch(ruta).set('Authorization', como(publicador))
      .send({ Mascota: { Nombre: 'Maximiliano' } }).expect(200);
    expect(body.Mascota.Nombre).toBe('Maximiliano');
    expect(body.Ubicacion.Latitud).toBeCloseTo(XALAPA.Latitud);
  });

  test('gestiona las solicitudes hasta aceptar una adopción', async () => {
    const rutaSolicitudes = `/api/adopciones/${adopcion.AdopcionID}/solicitudes`;

    await request(entorno.app).post(rutaSolicitudes).set('Authorization', como(publicador)).expect(400);
    const { body: solicitud } = await request(entorno.app).post(rutaSolicitudes).set('Authorization', como(interesado)).expect(201);
    await request(entorno.app).post(rutaSolicitudes).set('Authorization', como(interesado)).expect(409);

    await request(entorno.app).get(rutaSolicitudes).set('Authorization', como(interesado)).expect(403);
    const listado = await request(entorno.app).get(rutaSolicitudes).set('Authorization', como(publicador)).expect(200);
    expect(listado.body).toEqual([expect.objectContaining({ SolicitudID: solicitud.SolicitudID, NombreAdoptante: 'Luis' })]);

    await request(entorno.app).post(`${rutaSolicitudes}/${solicitud.SolicitudID}/aceptacion`).set('Authorization', como(publicador)).expect(204);

    const propias = await request(entorno.app).get('/api/adopciones/propias').set('Authorization', como(publicador)).expect(200);
    expect(propias.body[0]).toMatchObject({ AdopcionID: adopcion.AdopcionID, Estado: true });
    await expect(obtenerCercanas(entorno.clientesGrpc.ubicacion, interesado.token, CERCA_DE_XALAPA)).resolves.toEqual([]);

    const avisos = await request(entorno.app).get('/api/notificaciones').set('Authorization', como(interesado)).expect(200);
    expect(avisos.body.map((n) => n.Tipo)).toContain('SolicitudAceptada');
  });

  test('los reportes están reservados al administrador', async () => {
    await request(entorno.app).get('/api/adopciones?estado=adoptada').set('Authorization', como(publicador)).expect(403);
    const { body } = await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'admin.pruebas@adoptme.com', Contrasena: 'Administrador123' }).expect(200);
    const reporte = await request(entorno.app).get('/api/adopciones?estado=adoptada').set('Authorization', `Bearer ${body.token}`).expect(200);
    expect(reporte.body).toEqual([expect.objectContaining({ AdopcionID: adopcion.AdopcionID, Estado: true })]);
  });

  test('elimina la adopción y sus datos relacionados', async () => {
    const ruta = `/api/adopciones/${adopcion.AdopcionID}`;
    await request(entorno.app).delete(ruta).set('Authorization', como(interesado)).expect(403);
    await request(entorno.app).delete(ruta).set('Authorization', como(publicador)).expect(204);
    await request(entorno.app).get(ruta).set('Authorization', como(publicador)).expect(404);
  });
});
