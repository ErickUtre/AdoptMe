const esperar = (milisegundos) => new Promise((resolver) => setTimeout(resolver, milisegundos));

async function reintentar(operacion, { intentos, esperaMs, alFallar }) {
  let ultimoError;
  for (let intento = 1; intento <= intentos; intento += 1) {
    try {
      return await operacion();
    } catch (error) {
      ultimoError = error;
      alFallar?.(error, intento);
      if (intento < intentos) {
        await esperar(esperaMs);
      }
    }
  }
  throw ultimoError;
}

module.exports = { reintentar };
