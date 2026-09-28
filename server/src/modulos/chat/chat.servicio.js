const { ErrorNoEncontrado, ErrorValidacion } = require('../../compartido/errores');
const { exigirIdentificador, exigirTexto } = require('../../compartido/validacion');

const LONGITUD_MAXIMA_MENSAJE = 1000;

function mapearMensaje(mensaje) {
  return {
    ChatID: mensaje.ChatID,
    RemitenteID: mensaje.RemitenteID,
    DestinatarioID: mensaje.DestinatarioID,
    Contenido: mensaje.Contenido,
    FechaEnvio: mensaje.FechaEnvio
  };
}

function crearChatServicio({ chatRepositorio, usuarioRepositorio }) {
  async function enviar(remitenteId, datos) {
    const destinatarioId = exigirIdentificador(datos?.DestinatarioID, 'DestinatarioID');
    const contenido = exigirTexto(datos?.Contenido, 'Contenido', LONGITUD_MAXIMA_MENSAJE);
    if (destinatarioId === remitenteId) {
      throw new ErrorValidacion('No puedes enviarte mensajes a ti mismo');
    }
    if (!(await usuarioRepositorio.existe(destinatarioId))) {
      throw new ErrorNoEncontrado('El destinatario no existe');
    }
    const mensaje = await chatRepositorio.crear({
      RemitenteID: remitenteId,
      DestinatarioID: destinatarioId,
      Contenido: contenido
    });
    return mapearMensaje(mensaje);
  }

  async function listarConversaciones(usuarioId) {
    const mensajes = await chatRepositorio.listarDeUsuario(usuarioId);
    const conversaciones = new Map();

    for (const mensaje of mensajes) {
      const esPropio = mensaje.RemitenteID === usuarioId;
      const contraparte = esPropio ? mensaje.Destinatario : mensaje.Remitente;
      if (contraparte && !conversaciones.has(contraparte.UsuarioID)) {
        conversaciones.set(contraparte.UsuarioID, {
          UsuarioID: contraparte.UsuarioID,
          Nombre: contraparte.Nombre,
          UltimoMensaje: mensaje.Contenido,
          Fecha: mensaje.FechaEnvio
        });
      }
    }
    return [...conversaciones.values()];
  }

  async function listarMensajes(usuarioId, contraparteId) {
    const mensajes = await chatRepositorio.listarEntre(usuarioId, contraparteId);
    return mensajes.map(mapearMensaje);
  }

  return Object.freeze({ enviar, listarConversaciones, listarMensajes });
}

module.exports = { crearChatServicio };
