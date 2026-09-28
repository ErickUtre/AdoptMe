const { ConnectionError, UniqueConstraintError, ValidationError, ForeignKeyConstraintError } = require('sequelize');
const { ErrorAplicacion, ErrorNoEncontrado } = require('../../compartido/errores');

const REGLAS = Object.freeze([
  [(error) => error instanceof ErrorAplicacion, (error) => ({ estado: error.codigoHttp, mensaje: error.message })],
  [(error) => error instanceof UniqueConstraintError, () => ({ estado: 409, mensaje: 'El recurso ya existe' })],
  [(error) => error instanceof ForeignKeyConstraintError, () => ({ estado: 409, mensaje: 'El recurso está relacionado con otros datos' })],
  [(error) => error instanceof ValidationError, (error) => ({ estado: 400, mensaje: error.errors?.[0]?.message ?? 'Datos inválidos' })],
  [(error) => error instanceof ConnectionError, () => ({ estado: 503, mensaje: 'Error de conexión con la base de datos' })],
  [(error) => error?.type === 'entity.parse.failed', () => ({ estado: 400, mensaje: 'El cuerpo de la petición no es un JSON válido' })],
  [(error) => error?.type === 'entity.too.large', () => ({ estado: 413, mensaje: 'El cuerpo de la petición es demasiado grande' })]
]);

function traducirError(error) {
  const regla = REGLAS.find(([aplica]) => aplica(error));
  return regla ? regla[1](error) : null;
}

function crearManejadorErrores({ registro }) {
  return function manejarErrores(error, req, res, siguiente) {
    if (res.headersSent) {
      siguiente(error);
      return;
    }
    const traducido = traducirError(error);
    if (!traducido) {
      registro.error('Error no controlado', { ruta: req.originalUrl, metodo: req.method, error: error.message, pila: error.stack });
    }
    const { estado, mensaje } = traducido ?? { estado: 500, mensaje: 'Ocurrió un error en el servidor' };
    res.status(estado).json({ error: mensaje });
  };
}

function rutaNoEncontrada(req, res, siguiente) {
  siguiente(new ErrorNoEncontrado(`No existe la ruta ${req.method} ${req.originalUrl}`));
}

module.exports = { crearManejadorErrores, rutaNoEncontrada };
