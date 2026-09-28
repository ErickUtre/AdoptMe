const EVENTOS = Object.freeze({
  ENVIAR_MENSAJE: 'enviar_mensaje',
  NUEVO_MENSAJE: 'nuevo_mensaje',
  ERROR_MENSAJE: 'error_mensaje'
});

const salaDe = (usuarioId) => `usuario:${usuarioId}`;

function crearDifusorChat() {
  let servidor = null;

  return Object.freeze({
    vincular: (io) => {
      servidor = io;
    },
    difundir: (mensaje) => {
      servidor?.to(salaDe(mensaje.RemitenteID)).to(salaDe(mensaje.DestinatarioID)).emit(EVENTOS.NUEVO_MENSAJE, mensaje);
    }
  });
}

function extraerToken(socket) {
  const { auth = {}, headers = {} } = socket.handshake;
  if (typeof auth.token === 'string' && auth.token !== '') {
    return auth.token;
  }
  const encabezado = headers.authorization;
  return typeof encabezado === 'string' ? encabezado.replace(/^Bearer\s+/i, '') : '';
}

function registrarChatSocket({ io, tokens, chatServicio, difusorChat, registro }) {
  difusorChat.vincular(io);

  io.use((socket, siguiente) => {
    try {
      socket.data.sesion = tokens.verificar(extraerToken(socket));
      siguiente();
    } catch (error) {
      siguiente(error);
    }
  });

  io.on('connection', (socket) => {
    const { usuarioId } = socket.data.sesion;
    socket.join(salaDe(usuarioId));

    socket.on(EVENTOS.ENVIAR_MENSAJE, async (carga) => {
      try {
        const datos = typeof carga === 'string' ? JSON.parse(carga) : carga;
        difusorChat.difundir(await chatServicio.enviar(usuarioId, datos));
      } catch (error) {
        registro.advertencia('Mensaje de chat rechazado', { usuarioId, causa: error.message });
        socket.emit(EVENTOS.ERROR_MENSAJE, { mensaje: error.codigoHttp ? error.message : 'Error al enviar el mensaje' });
      }
    });
  });
}

module.exports = { registrarChatSocket, crearDifusorChat, EVENTOS_CHAT: EVENTOS };
