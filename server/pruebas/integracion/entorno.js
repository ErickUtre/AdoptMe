const os = require('node:os');
const path = require('node:path');

process.env.NODE_ENV = 'test';
process.env.PORT = '0';
process.env.GRPC_PORT = '0';
process.env.JWT_SECRET ??= 'secreto-de-integracion';
process.env.DB_CONNECT_RETRIES ??= '30';
process.env.DB_CONNECT_RETRY_DELAY_MS ??= '3000';
process.env.MEDIA_DIR = path.join(os.tmpdir(), 'adoptme-multimedia-pruebas');
process.env.ADMIN_EMAIL = 'admin.pruebas@adoptme.com';
process.env.ADMIN_PASSWORD = 'Administrador123';
process.env.ADMIN_NAME = 'Administrador de pruebas';
