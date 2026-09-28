const fs = require('node:fs');
const path = require('node:path');
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const swaggerUi = require('swagger-ui-express');
const YAML = require('yaml');
const { crearManejadorErrores, rutaNoEncontrada } = require('./middlewares/manejarErrores');

const DOCUMENTO_API = YAML.parse(fs.readFileSync(path.join(__dirname, 'openapi.yaml'), 'utf8'));

function crearAplicacion({ modulosHttp, origenesPermitidos, registro }) {
  const app = express();

  app.disable('x-powered-by');
  app.set('trust proxy', 1);
  app.use(helmet({ contentSecurityPolicy: false }));
  app.use(cors({ origin: origenesPermitidos === '*' ? '*' : origenesPermitidos.split(',') }));
  app.use(express.json({ limit: '100kb' }));

  app.get('/salud', (req, res) => res.json({ estado: 'ok' }));
  app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(DOCUMENTO_API));

  for (const { ruta, enrutador } of modulosHttp) {
    app.use(ruta, enrutador);
  }

  app.use(rutaNoEncontrada);
  app.use(crearManejadorErrores({ registro }));

  return app;
}

module.exports = { crearAplicacion };
