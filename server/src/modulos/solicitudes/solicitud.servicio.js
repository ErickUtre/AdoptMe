const { ErrorConflicto, ErrorNoEncontrado, ErrorProhibido, ErrorValidacion } = require('../../compartido/errores');
const { TIPOS_NOTIFICACION, REFERENCIAS_NOTIFICACION } = require('../notificaciones/notificacion.servicio');

function mapearSolicitud(solicitud) {
  return {
    SolicitudID: solicitud.SolicitudID,
    AdopcionID: solicitud.AdopcionID,
    AdoptanteID: solicitud.AdoptanteID,
    NombreAdoptante: solicitud.Adoptante?.Nombre ?? 'Desconocido'
  };
}

function crearSolicitudServicio({ solicitudRepositorio, adopcionServicio, notificacionServicio, transaccion, registro }) {
  async function exigirSolicitudDeAdopcion(solicitudId, adopcionId, transaction) {
    const solicitud = await solicitudRepositorio.buscarPorId(solicitudId, transaction);
    if (!solicitud || solicitud.AdopcionID !== adopcionId) {
      throw new ErrorNoEncontrado('Solicitud no encontrada');
    }
    return solicitud;
  }

  async function notificarSinInterrumpir(usuarioId, contenido) {
    try {
      await notificacionServicio.notificar(usuarioId, contenido);
    } catch (error) {
      registro.error('No se pudo enviar la notificación', { usuarioId, causa: error.message });
    }
  }

  async function registrar(sesion, adopcionId) {
    const adopcion = await adopcionServicio.exigirAdopcion(adopcionId);
    if (adopcion.PublicadorID === sesion.usuarioId) {
      throw new ErrorValidacion('No puedes solicitar tu propia adopción');
    }
    if (adopcion.Estado) {
      throw new ErrorConflicto('La mascota ya fue adoptada');
    }
    if (await solicitudRepositorio.existe(sesion.usuarioId, adopcionId)) {
      throw new ErrorConflicto('Ya has enviado una solicitud para esta adopción');
    }

    const solicitud = await solicitudRepositorio.crear({ AdoptanteID: sesion.usuarioId, AdopcionID: adopcionId });
    await notificarSinInterrumpir(adopcion.PublicadorID, {
      titulo: 'Nueva solicitud de adopción',
      mensaje: 'Alguien quiere adoptar a tu mascota. Revisa tus solicitudes pendientes.',
      tipo: TIPOS_NOTIFICACION.SOLICITUD_RECIBIDA,
      referenciaId: adopcionId,
      referenciaTipo: REFERENCIAS_NOTIFICACION.ADOPCION
    });

    return { SolicitudID: solicitud.SolicitudID, AdopcionID: adopcionId, AdoptanteID: sesion.usuarioId };
  }

  async function listar(sesion, adopcionId) {
    await adopcionServicio.exigirPropia(sesion, adopcionId);
    const solicitudes = await solicitudRepositorio.listarPorAdopcion(adopcionId);
    return solicitudes.map(mapearSolicitud);
  }

  async function rechazar(sesion, adopcionId, solicitudId) {
    const adopcion = await adopcionServicio.exigirAdopcion(adopcionId);
    const solicitud = await exigirSolicitudDeAdopcion(solicitudId, adopcionId);
    const esPublicador = adopcion.PublicadorID === sesion.usuarioId;
    const esAdoptante = solicitud.AdoptanteID === sesion.usuarioId;
    if (!esPublicador && !esAdoptante) {
      throw new ErrorProhibido('No puedes modificar esta solicitud');
    }
    await solicitudRepositorio.eliminar(solicitudId);
  }

  async function aceptar(sesion, adopcionId, solicitudId) {
    const solicitud = await transaccion(async (transaction) => {
      const adopcion = await adopcionServicio.exigirPropia(sesion, adopcionId, transaction);
      if (adopcion.Estado) {
        throw new ErrorConflicto('La mascota ya fue adoptada');
      }
      const aceptada = await exigirSolicitudDeAdopcion(solicitudId, adopcionId, transaction);
      await adopcionServicio.marcarAdoptada(adopcionId, transaction);
      await solicitudRepositorio.eliminarPorAdopcion(adopcionId, transaction);
      return aceptada;
    });

    await adopcionServicio.retirarDelMapa(adopcionId);
    await notificarSinInterrumpir(solicitud.AdoptanteID, {
      titulo: 'Solicitud aceptada',
      mensaje: '¡Tu solicitud de adopción fue aceptada! Ponte en contacto con el publicador.',
      tipo: TIPOS_NOTIFICACION.SOLICITUD_ACEPTADA,
      referenciaId: adopcionId,
      referenciaTipo: REFERENCIAS_NOTIFICACION.ADOPCION
    });
  }

  return Object.freeze({ registrar, listar, rechazar, aceptar });
}

module.exports = { crearSolicitudServicio };
