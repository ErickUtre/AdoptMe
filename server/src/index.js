const fs = require('node:fs');
const path = require('node:path');
const { cargarEntorno } = require('./config/entorno');
const { crearRegistro } = require('./compartido/registro');
const { prepararServidor } = require('./servidor');

const ARCHIVO_ENTORNO = path.resolve(__dirname, '..', '..', '.env');

async function principal() {
  if (fs.existsSync(ARCHIVO_ENTORNO)) {
    process.loadEnvFile(ARCHIVO_ENTORNO);
  }

  const configuracion = cargarEntorno();
  const registro = crearRegistro({ nivel: process.env.LOG_LEVEL ?? 'info' });
  const servidor = await prepararServidor({ configuracion, registro });
  await servidor.iniciar();

  const apagar = async (senal) => {
    registro.info('Deteniendo el servidor', { senal });
    await servidor.detener();
    process.exit(0);
  };
  process.once('SIGTERM', apagar);
  process.once('SIGINT', apagar);
}

principal().catch((error) => {
  console.error(error);
  process.exit(1);
});
