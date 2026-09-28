function crearAdopcionGrpc({ adopcionServicio, autenticacionGrpc }) {
  return {
    ObtenerAdopcionesCercanas: autenticacionGrpc.protegerUnaria(async (llamada) => ({
      resultados: await adopcionServicio.buscarCercanas(llamada.sesion, llamada.request)
    }))
  };
}

module.exports = { crearAdopcionGrpc };
