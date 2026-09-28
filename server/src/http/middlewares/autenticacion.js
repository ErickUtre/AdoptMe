const { ErrorProhibido } = require('../../compartido/errores');

function crearAutenticacionHttp({ tokens }) {
  function requerirSesion(req, res, siguiente) {
    req.sesion = tokens.verificarEncabezado(req.headers.authorization);
    siguiente();
  }

  function requerirRol(rol) {
    return function verificarRol(req, res, siguiente) {
      if (req.sesion?.rol !== rol) {
        throw new ErrorProhibido();
      }
      siguiente();
    };
  }

  return Object.freeze({ requerirSesion, requerirRol });
}

module.exports = { crearAutenticacionHttp };
