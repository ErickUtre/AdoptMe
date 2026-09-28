const jwt = require('jsonwebtoken');
const { ErrorNoAutenticado } = require('../../compartido/errores');

const PREFIJO_BEARER = 'Bearer ';

function crearServicioTokens({ secreto, expiracion }) {
  function emitir(sesion) {
    return jwt.sign(
      { UsuarioID: sesion.usuarioId, rol: sesion.rol },
      secreto,
      { expiresIn: expiracion, subject: String(sesion.usuarioId) }
    );
  }

  function verificar(token) {
    try {
      const contenido = jwt.verify(token, secreto);
      return Object.freeze({ usuarioId: contenido.UsuarioID, rol: contenido.rol });
    } catch {
      throw new ErrorNoAutenticado('Token inválido o expirado');
    }
  }

  function verificarEncabezado(encabezado) {
    if (typeof encabezado !== 'string' || !encabezado.startsWith(PREFIJO_BEARER)) {
      throw new ErrorNoAutenticado('Token no proporcionado');
    }
    return verificar(encabezado.slice(PREFIJO_BEARER.length).trim());
  }

  return Object.freeze({ emitir, verificar, verificarEncabezado });
}

module.exports = { crearServicioTokens };
