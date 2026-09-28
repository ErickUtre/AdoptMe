const NIVELES = Object.freeze({ depuracion: 10, info: 20, advertencia: 30, error: 40, silencio: 100 });

function crearRegistro({ nivel = 'info', salida = console } = {}) {
  const umbral = NIVELES[nivel] ?? NIVELES.info;

  const escribir = (nombreNivel, metodo) => (mensaje, contexto = {}) => {
    if (NIVELES[nombreNivel] < umbral) {
      return;
    }
    const detalle = contexto instanceof Error
      ? { error: contexto.message, pila: contexto.stack }
      : contexto;
    salida[metodo](JSON.stringify({ fecha: new Date().toISOString(), nivel: nombreNivel, mensaje, ...detalle }));
  };

  return Object.freeze({
    depuracion: escribir('depuracion', 'debug'),
    info: escribir('info', 'info'),
    advertencia: escribir('advertencia', 'warn'),
    error: escribir('error', 'error')
  });
}

module.exports = { crearRegistro };
