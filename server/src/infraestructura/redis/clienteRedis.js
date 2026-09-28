const { createClient } = require('redis');
const { reintentar } = require('../../compartido/reintentar');

async function conectarRedis(configuracion, registro) {
  const cliente = createClient({ url: configuracion.url, database: configuracion.baseDatos });
  cliente.on('error', (error) => registro.error('Error en el cliente Redis', error));

  await reintentar(() => cliente.connect(), {
    intentos: 10,
    esperaMs: 3000,
    alFallar: (error, intento) => registro.advertencia('Redis no disponible', { intento, causa: error.message })
  });

  registro.info('Conexión a Redis establecida');
  return cliente;
}

module.exports = { conectarRedis };
