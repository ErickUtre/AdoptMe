const { exigirIdentificador } = require('../../compartido/validacion');

function crearNotificacionControlador({ notificacionServicio }) {
  return Object.freeze({
    listar: async (req, res) => {
      res.json(await notificacionServicio.listar(req.sesion.usuarioId));
    },

    eliminar: async (req, res) => {
      await notificacionServicio.eliminar(req.sesion.usuarioId, exigirIdentificador(req.params.id, 'id'));
      res.status(204).end();
    },

    eliminarTodas: async (req, res) => {
      await notificacionServicio.eliminarTodas(req.sesion.usuarioId);
      res.status(204).end();
    }
  });
}

module.exports = { crearNotificacionControlador };
