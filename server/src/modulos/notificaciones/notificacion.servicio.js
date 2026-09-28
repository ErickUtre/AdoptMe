const { ErrorNoEncontrado } = require('../../compartido/errores');

const TIPOS = Object.freeze({
  ADOPCION_CERCANA: 'AdopcionCercana',
  SOLICITUD_RECIBIDA: 'SolicitudRecibida',
  SOLICITUD_ACEPTADA: 'SolicitudAceptada'
});

const REFERENCIAS = Object.freeze({ ADOPCION: 'Adopcion' });

function mapearParaDifusion(notificacion) {
  return {
    notificacionId: notificacion.NotificacionID,
    titulo: notificacion.Titulo,
    mensaje: notificacion.Mensaje,
    tipo: notificacion.Tipo,
    referenciaId: notificacion.ReferenciaID ?? 0,
    referenciaTipo: notificacion.ReferenciaTipo ?? '',
    fecha: (notificacion.FechaCreacion ? new Date(notificacion.FechaCreacion) : new Date()).toISOString()
  };
}

function crearNotificacionServicio({ notificacionRepositorio, difusor }) {
  function aRegistro(usuarioId, { titulo, mensaje, tipo, referenciaId = null, referenciaTipo = null }) {
    return {
      UsuarioID: usuarioId,
      Titulo: titulo,
      Mensaje: mensaje,
      Tipo: tipo,
      ReferenciaID: referenciaId,
      ReferenciaTipo: referenciaTipo,
      Leida: false
    };
  }

  async function notificar(usuarioId, contenido) {
    const creada = await notificacionRepositorio.crear(aRegistro(usuarioId, contenido));
    difusor.publicar(usuarioId, mapearParaDifusion(creada));
    return creada;
  }

  async function notificarVarios(destinatarios) {
    if (destinatarios.length === 0) {
      return;
    }
    const creadas = await notificacionRepositorio.crearVarias(
      destinatarios.map(({ usuarioId, contenido }) => aRegistro(usuarioId, contenido))
    );
    creadas.forEach((notificacion) => difusor.publicar(notificacion.UsuarioID, mapearParaDifusion(notificacion)));
  }

  async function eliminar(usuarioId, notificacionId) {
    const eliminadas = await notificacionRepositorio.eliminar(usuarioId, notificacionId);
    if (eliminadas === 0) {
      throw new ErrorNoEncontrado('Notificación no encontrada');
    }
  }

  return Object.freeze({
    notificar,
    notificarVarios,
    listar: (usuarioId) => notificacionRepositorio.listarPorUsuario(usuarioId),
    eliminar,
    eliminarTodas: (usuarioId) => notificacionRepositorio.eliminarTodas(usuarioId),
    suscribir: (usuarioId, alRecibir) => difusor.suscribir(usuarioId, alRecibir)
  });
}

module.exports = { crearNotificacionServicio, TIPOS_NOTIFICACION: TIPOS, REFERENCIAS_NOTIFICACION: REFERENCIAS };
