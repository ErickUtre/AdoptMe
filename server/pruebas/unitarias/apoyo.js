const registroSilencioso = Object.freeze({
  depuracion: () => {},
  info: () => {},
  advertencia: () => {},
  error: () => {}
});

const transaccionDirecta = (trabajo) => trabajo('transaccion');

function crearIndiceGeograficoFalso({ usuarios = [], adopciones = [] } = {}) {
  return {
    registrarUsuario: jest.fn().mockResolvedValue(),
    eliminarUsuario: jest.fn().mockResolvedValue(),
    buscarUsuarios: jest.fn().mockResolvedValue(usuarios),
    registrarAdopcion: jest.fn().mockResolvedValue(),
    eliminarAdopcion: jest.fn().mockResolvedValue(),
    buscarAdopciones: jest.fn().mockResolvedValue(adopciones)
  };
}

module.exports = { registroSilencioso, transaccionDirecta, crearIndiceGeograficoFalso };
