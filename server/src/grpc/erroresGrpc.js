const { status } = require('@grpc/grpc-js');
const { ErrorAplicacion } = require('../compartido/errores');

const ESTADO_POR_CODIGO_HTTP = Object.freeze({
  400: status.INVALID_ARGUMENT,
  401: status.UNAUTHENTICATED,
  403: status.PERMISSION_DENIED,
  404: status.NOT_FOUND,
  409: status.ALREADY_EXISTS,
  503: status.UNAVAILABLE
});

function aErrorGrpc(error, registro) {
  if (error instanceof ErrorAplicacion) {
    return { code: ESTADO_POR_CODIGO_HTTP[error.codigoHttp] ?? status.UNKNOWN, details: error.message };
  }
  registro.error('Error no controlado en gRPC', error);
  return { code: status.INTERNAL, details: 'Error interno del servidor' };
}

module.exports = { aErrorGrpc };
