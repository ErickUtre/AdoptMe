const { exigirIdentificador } = require('../../compartido/validacion');

function crearSolicitudControlador({ solicitudServicio }) {
  const idAdopcion = (req) => exigirIdentificador(req.params.adopcionId, 'adopcionId');
  const idSolicitud = (req) => exigirIdentificador(req.params.solicitudId, 'solicitudId');

  return Object.freeze({
    registrar: async (req, res) => {
      res.status(201).json(await solicitudServicio.registrar(req.sesion, idAdopcion(req)));
    },

    listar: async (req, res) => {
      res.json(await solicitudServicio.listar(req.sesion, idAdopcion(req)));
    },

    aceptar: async (req, res) => {
      await solicitudServicio.aceptar(req.sesion, idAdopcion(req), idSolicitud(req));
      res.status(204).end();
    },

    rechazar: async (req, res) => {
      await solicitudServicio.rechazar(req.sesion, idAdopcion(req), idSolicitud(req));
      res.status(204).end();
    }
  });
}

module.exports = { crearSolicitudControlador };
