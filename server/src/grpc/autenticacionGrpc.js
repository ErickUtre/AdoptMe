const { aErrorGrpc } = require('./erroresGrpc');

function crearAutenticacionGrpc({ tokens, registro }) {
  function autenticar(llamada) {
    const [encabezado] = llamada.metadata.get('authorization');
    llamada.sesion = tokens.verificarEncabezado(encabezado);
  }

  function protegerUnaria(manejador) {
    return async (llamada, responder) => {
      try {
        autenticar(llamada);
        responder(null, await manejador(llamada));
      } catch (error) {
        responder(aErrorGrpc(error, registro));
      }
    };
  }

  function protegerFlujoSalida(manejador) {
    return (llamada) => {
      try {
        autenticar(llamada);
        manejador(llamada);
      } catch (error) {
        llamada.emit('error', aErrorGrpc(error, registro));
      }
    };
  }

  return Object.freeze({ protegerUnaria, protegerFlujoSalida });
}

module.exports = { crearAutenticacionGrpc };
