const { exigirIdentificador } = require('../../compartido/validacion');

function crearAdopcionControlador({ adopcionServicio }) {
  const idAdopcion = (req) => exigirIdentificador(req.params.id, 'id');

  return Object.freeze({
    registrar: async (req, res) => {
      res.status(201).json(await adopcionServicio.registrar(req.sesion, req.body));
    },

    listarParaReporte: async (req, res) => {
      res.json(await adopcionServicio.listarParaReporte(req.query.estado));
    },

    listarPropias: async (req, res) => {
      res.json(await adopcionServicio.listarPropias(req.sesion));
    },

    obtenerDetalle: async (req, res) => {
      res.json(await adopcionServicio.obtenerDetalle(idAdopcion(req)));
    },

    modificar: async (req, res) => {
      res.json(await adopcionServicio.modificar(req.sesion, idAdopcion(req), req.body));
    },

    eliminar: async (req, res) => {
      await adopcionServicio.eliminar(req.sesion, idAdopcion(req));
      res.status(204).end();
    }
  });
}

module.exports = { crearAdopcionControlador };
