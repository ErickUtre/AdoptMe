function crearAutenticacionControlador({ autenticacionServicio }) {
  return Object.freeze({
    iniciarSesion: async (req, res) => {
      res.json(await autenticacionServicio.iniciarSesion(req.body));
    },

    actualizarCredenciales: async (req, res) => {
      await autenticacionServicio.actualizarCredenciales(req.sesion.usuarioId, req.body);
      res.status(204).end();
    }
  });
}

module.exports = { crearAutenticacionControlador };
