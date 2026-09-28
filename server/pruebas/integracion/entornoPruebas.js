const path = require('node:path');
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const request = require('supertest');
const { cargarEntorno } = require('../../src/config/entorno');
const { crearRegistro } = require('../../src/compartido/registro');
const { prepararServidor } = require('../../src/servidor');

const TABLAS_EN_ORDEN_DE_BORRADO = Object.freeze([
  'Notificacion', 'Chat', 'Solicitud', 'FotoMascota', 'VideoMascota', 'FotoUsuario',
  'Adopcion', 'Mascota', 'Usuario', 'Acceso', 'Ubicacion'
]);

const XALAPA = Object.freeze({ Latitud: 19.54162, Longitud: -96.932527, Ciudad: 'Xalapa', Estado: 'Veracruz', Pais: 'México' });
const CERCA_DE_XALAPA = Object.freeze({ Latitud: 19.54562, Longitud: -96.930527, Ciudad: 'Xalapa', Estado: 'Veracruz', Pais: 'México' });

async function limpiarDatos({ baseDatos, redis }) {
  for (const tabla of TABLAS_EN_ORDEN_DE_BORRADO) {
    await baseDatos.sequelize.query(`DELETE FROM dbo.${tabla}`);
  }
  await redis.flushDb();
}

function crearClientesGrpc(configuracion, puerto) {
  const cargar = (archivo, paquete, servicio) => {
    const definicion = protoLoader.loadSync(path.join(configuracion.grpc.rutaProtos, archivo), {
      keepCase: false, longs: Number, enums: String, defaults: true, oneofs: true
    });
    const Cliente = grpc.loadPackageDefinition(definicion)[paquete][servicio];
    return new Cliente(`127.0.0.1:${puerto}`, grpc.credentials.createInsecure());
  };
  return {
    ubicacion: cargar('ubicacion.proto', 'ubicacion', 'ServicioUbicacion'),
    multimedia: cargar('multimedia.proto', 'multimedia', 'ServicioMultimedia'),
    notificacion: cargar('notificacion.proto', 'notificacion', 'ServicioNotificacion')
  };
}

function metadatosDe(token) {
  const metadatos = new grpc.Metadata();
  metadatos.add('authorization', `Bearer ${token}`);
  return metadatos;
}

async function levantarEntorno() {
  const configuracion = cargarEntorno();
  const servidor = await prepararServidor({ configuracion, registro: crearRegistro({ nivel: 'silencio' }) });
  await limpiarDatos(servidor);
  const puertos = await servidor.iniciar();
  const clientesGrpc = crearClientesGrpc(configuracion, puertos.puertoGrpc);

  async function registrarYEntrar({ nombre, correo, ubicacion }) {
    await request(servidor.app).post('/api/usuarios').send({
      Nombre: nombre,
      Telefono: '2281234567',
      Ubicacion: ubicacion,
      Acceso: { Correo: correo, Contrasena: 'Segura123' }
    }).expect(201);
    const { body } = await request(servidor.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: correo, Contrasena: 'Segura123' }).expect(200);
    return { token: body.token, usuario: body.usuario };
  }

  async function cerrar() {
    Object.values(clientesGrpc).forEach((cliente) => cliente.close());
    await servidor.detener();
  }

  return { servidor, app: servidor.app, puertos, clientesGrpc, registrarYEntrar, cerrar };
}

module.exports = { levantarEntorno, metadatosDe, grpc, XALAPA, CERCA_DE_XALAPA };
