const { exigirIdentificador } = require('../../compartido/validacion');

function crearChatControlador({ chatServicio, difusorChat }) {
  const idContraparte = (req) => exigirIdentificador(req.params.usuarioId, 'usuarioId');

  return Object.freeze({
    listarConversaciones: async (req, res) => {
      res.json(await chatServicio.listarConversaciones(req.sesion.usuarioId));
    },

    listarMensajes: async (req, res) => {
      res.json(await chatServicio.listarMensajes(req.sesion.usuarioId, idContraparte(req)));
    },

    enviar: async (req, res) => {
      const mensaje = await chatServicio.enviar(req.sesion.usuarioId, {
        DestinatarioID: idContraparte(req),
        Contenido: req.body?.Contenido
      });
      difusorChat.difundir(mensaje);
      res.status(201).json(mensaje);
    }
  });
}

module.exports = { crearChatControlador };
