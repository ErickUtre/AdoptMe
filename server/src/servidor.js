const http = require('node:http');
const { Server: ServidorSocket } = require('socket.io');
const { conectarBaseDatos } = require('./infraestructura/baseDatos/conexion');
const { conectarRedis } = require('./infraestructura/redis/clienteRedis');
const { crearContenedor } = require('./contenedor');
const { crearAplicacion } = require('./http/aplicacion');
const { crearServidorGrpc } = require('./grpc/servidorGrpc');
const { registrarChatSocket } = require('./modulos/chat/chat.socket');
const { sembrarAdministrador } = require('./arranque/sembrarAdministrador');
const { sincronizarIndiceGeografico } = require('./arranque/sincronizarIndiceGeografico');

async function prepararServidor({ configuracion, registro }) {
  const baseDatos = await conectarBaseDatos(configuracion.baseDatos, registro);
  const redis = await conectarRedis(configuracion.redis, registro);
  const contenedor = crearContenedor({ configuracion, baseDatos, redis, registro });

  const app = crearAplicacion({
    modulosHttp: contenedor.modulosHttp,
    origenesPermitidos: configuracion.http.origenesPermitidos,
    registro
  });
  const servidorHttp = http.createServer(app);
  const io = new ServidorSocket(servidorHttp, { cors: { origin: configuracion.http.origenesPermitidos } });
  registrarChatSocket({
    io, tokens: contenedor.tokens, chatServicio: contenedor.chatServicio, difusorChat: contenedor.difusorChat, registro
  });
  const servidorGrpc = crearServidorGrpc({
    rutaProtos: configuracion.grpc.rutaProtos, servicios: contenedor.serviciosGrpc, registro
  });

  async function iniciar() {
    await sembrarAdministrador({
      configuracion: configuracion.administrador,
      usuarioRepositorio: contenedor.repositorios.usuario,
      contrasenas: contenedor.contrasenas,
      transaccion: baseDatos.transaccion,
      registro
    });
    await sincronizarIndiceGeografico({
      usuarioRepositorio: contenedor.repositorios.usuario,
      adopcionRepositorio: contenedor.repositorios.adopcion,
      indiceGeografico: contenedor.indiceGeografico,
      registro
    });
    const puertoGrpc = await servidorGrpc.iniciar(configuracion.grpc.puerto);
    await new Promise((resolver) => servidorHttp.listen(configuracion.http.puerto, resolver));
    const puertoHttp = servidorHttp.address().port;
    registro.info('Servidor HTTP escuchando', { puerto: puertoHttp });
    return { puertoHttp, puertoGrpc };
  }

  async function detener() {
    io.close();
    await servidorGrpc.detener();
    await new Promise((resolver) => servidorHttp.close(() => resolver()));
    await redis.quit();
    await baseDatos.cerrar();
  }

  return Object.freeze({ app, baseDatos, redis, contenedor, iniciar, detener });
}

module.exports = { prepararServidor };
