const { crearServicioTokens } = require('./infraestructura/seguridad/tokens');
const { crearServicioContrasenas } = require('./infraestructura/seguridad/contrasenas');
const { crearAlmacenamientoArchivos } = require('./infraestructura/almacenamiento/almacenamientoArchivos');
const { crearAutenticacionHttp } = require('./http/middlewares/autenticacion');
const { crearAutenticacionGrpc } = require('./grpc/autenticacionGrpc');

const { crearIndiceGeografico } = require('./modulos/ubicaciones/indiceGeografico');
const { crearUbicacionRepositorio } = require('./modulos/ubicaciones/ubicacion.repositorio');

const { crearUsuarioRepositorio } = require('./modulos/usuarios/usuario.repositorio');
const { crearUsuarioServicio } = require('./modulos/usuarios/usuario.servicio');
const { crearUsuarioControlador } = require('./modulos/usuarios/usuario.controlador');
const { crearUsuarioRutas } = require('./modulos/usuarios/usuario.rutas');

const { crearAutenticacionServicio } = require('./modulos/autenticacion/autenticacion.servicio');
const { crearAutenticacionControlador } = require('./modulos/autenticacion/autenticacion.controlador');
const { crearAutenticacionRutas } = require('./modulos/autenticacion/autenticacion.rutas');

const { crearDifusorNotificaciones } = require('./modulos/notificaciones/difusorNotificaciones');
const { crearNotificacionRepositorio } = require('./modulos/notificaciones/notificacion.repositorio');
const { crearNotificacionServicio } = require('./modulos/notificaciones/notificacion.servicio');
const { crearNotificacionControlador } = require('./modulos/notificaciones/notificacion.controlador');
const { crearNotificacionRutas } = require('./modulos/notificaciones/notificacion.rutas');
const { crearNotificacionGrpc } = require('./modulos/notificaciones/notificacion.grpc');

const { crearMultimediaRepositorio } = require('./modulos/multimedia/multimedia.repositorio');
const { crearMultimediaServicio } = require('./modulos/multimedia/multimedia.servicio');
const { crearMultimediaControlador } = require('./modulos/multimedia/multimedia.controlador');
const { crearMultimediaRutas } = require('./modulos/multimedia/multimedia.rutas');
const { crearMultimediaGrpc } = require('./modulos/multimedia/multimedia.grpc');

const { crearAdopcionRepositorio } = require('./modulos/adopciones/adopcion.repositorio');
const { crearAdopcionServicio } = require('./modulos/adopciones/adopcion.servicio');
const { crearAdopcionControlador } = require('./modulos/adopciones/adopcion.controlador');
const { crearAdopcionRutas } = require('./modulos/adopciones/adopcion.rutas');
const { crearAdopcionGrpc } = require('./modulos/adopciones/adopcion.grpc');

const { crearSolicitudRepositorio } = require('./modulos/solicitudes/solicitud.repositorio');
const { crearSolicitudServicio } = require('./modulos/solicitudes/solicitud.servicio');
const { crearSolicitudControlador } = require('./modulos/solicitudes/solicitud.controlador');
const { crearSolicitudRutas } = require('./modulos/solicitudes/solicitud.rutas');

const { crearChatRepositorio } = require('./modulos/chat/chat.repositorio');
const { crearChatServicio } = require('./modulos/chat/chat.servicio');
const { crearChatControlador } = require('./modulos/chat/chat.controlador');
const { crearChatRutas } = require('./modulos/chat/chat.rutas');
const { crearDifusorChat } = require('./modulos/chat/chat.socket');

function crearContenedor({ configuracion, baseDatos, redis, registro }) {
  const { modelos, transaccion } = baseDatos;

  const tokens = crearServicioTokens(configuracion.jwt);
  const contrasenas = crearServicioContrasenas();
  const almacenamiento = crearAlmacenamientoArchivos({ directorioRaiz: configuracion.multimedia.directorio });
  const autenticacion = crearAutenticacionHttp({ tokens });
  const autenticacionGrpc = crearAutenticacionGrpc({ tokens, registro });
  const indiceGeografico = crearIndiceGeografico(redis);

  const repositorios = {
    ubicacion: crearUbicacionRepositorio(modelos),
    usuario: crearUsuarioRepositorio(modelos),
    notificacion: crearNotificacionRepositorio(modelos),
    multimedia: crearMultimediaRepositorio(modelos),
    adopcion: crearAdopcionRepositorio(modelos),
    solicitud: crearSolicitudRepositorio(modelos),
    chat: crearChatRepositorio(modelos)
  };

  const usuarioServicio = crearUsuarioServicio({
    usuarioRepositorio: repositorios.usuario,
    ubicacionRepositorio: repositorios.ubicacion,
    indiceGeografico,
    contrasenas,
    transaccion,
    registro
  });
  const autenticacionServicio = crearAutenticacionServicio({
    usuarioRepositorio: repositorios.usuario, contrasenas, tokens, transaccion
  });
  const notificacionServicio = crearNotificacionServicio({
    notificacionRepositorio: repositorios.notificacion, difusor: crearDifusorNotificaciones()
  });
  const multimediaServicio = crearMultimediaServicio({
    multimediaRepositorio: repositorios.multimedia,
    adopcionRepositorio: repositorios.adopcion,
    almacenamiento,
    transaccion,
    registro
  });
  const adopcionServicio = crearAdopcionServicio({
    adopcionRepositorio: repositorios.adopcion,
    indiceGeografico,
    notificacionServicio,
    multimediaServicio,
    transaccion,
    registro
  });
  const solicitudServicio = crearSolicitudServicio({
    solicitudRepositorio: repositorios.solicitud, adopcionServicio, notificacionServicio, transaccion, registro
  });
  const chatServicio = crearChatServicio({ chatRepositorio: repositorios.chat, usuarioRepositorio: repositorios.usuario });
  const difusorChat = crearDifusorChat();

  const modulosHttp = [
    {
      ruta: '/api/acceso',
      enrutador: crearAutenticacionRutas({
        autenticacionControlador: crearAutenticacionControlador({ autenticacionServicio }), autenticacion
      })
    },
    {
      ruta: '/api',
      enrutador: crearMultimediaRutas({ multimediaControlador: crearMultimediaControlador({ multimediaServicio }), autenticacion })
    },
    {
      ruta: '/api/usuarios',
      enrutador: crearUsuarioRutas({ usuarioControlador: crearUsuarioControlador({ usuarioServicio }), autenticacion })
    },
    {
      ruta: '/api/adopciones/:adopcionId/solicitudes',
      enrutador: crearSolicitudRutas({ solicitudControlador: crearSolicitudControlador({ solicitudServicio }), autenticacion })
    },
    {
      ruta: '/api/adopciones',
      enrutador: crearAdopcionRutas({ adopcionControlador: crearAdopcionControlador({ adopcionServicio }), autenticacion })
    },
    {
      ruta: '/api/chat',
      enrutador: crearChatRutas({ chatControlador: crearChatControlador({ chatServicio, difusorChat }), autenticacion })
    },
    {
      ruta: '/api/notificaciones',
      enrutador: crearNotificacionRutas({ notificacionControlador: crearNotificacionControlador({ notificacionServicio }), autenticacion })
    }
  ];

  const serviciosGrpc = [
    {
      archivo: 'ubicacion.proto',
      paquete: 'ubicacion',
      servicio: 'ServicioUbicacion',
      implementacion: crearAdopcionGrpc({ adopcionServicio, autenticacionGrpc })
    },
    {
      archivo: 'multimedia.proto',
      paquete: 'multimedia',
      servicio: 'ServicioMultimedia',
      implementacion: crearMultimediaGrpc({ multimediaServicio, autenticacionGrpc })
    },
    {
      archivo: 'notificacion.proto',
      paquete: 'notificacion',
      servicio: 'ServicioNotificacion',
      implementacion: crearNotificacionGrpc({ notificacionServicio, autenticacionGrpc })
    }
  ];

  return Object.freeze({
    tokens,
    contrasenas,
    indiceGeografico,
    repositorios,
    chatServicio,
    difusorChat,
    modulosHttp,
    serviciosGrpc
  });
}

module.exports = { crearContenedor };
