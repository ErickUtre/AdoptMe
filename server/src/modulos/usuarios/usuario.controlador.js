const { exigirIdentificador } = require('../../compartido/validacion');

function crearUsuarioControlador({ usuarioServicio }) {
  return Object.freeze({
    registrar: async (req, res) => {
      res.status(201).json(await usuarioServicio.registrar(req.body));
    },

    obtenerPerfilPropio: async (req, res) => {
      res.json(await usuarioServicio.obtenerPerfil(req.sesion.usuarioId));
    },

    actualizarPerfilPropio: async (req, res) => {
      res.json(await usuarioServicio.actualizarPerfil(req.sesion.usuarioId, req.body));
    },

    actualizarUbicacionPropia: async (req, res) => {
      res.json(await usuarioServicio.actualizarUbicacion(req.sesion.usuarioId, req.body));
    },

    obtenerResumen: async (req, res) => {
      res.json(await usuarioServicio.obtenerResumen(exigirIdentificador(req.params.id, 'id')));
    }
  });
}

module.exports = { crearUsuarioControlador };
