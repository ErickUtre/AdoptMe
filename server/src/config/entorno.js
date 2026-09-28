const path = require('node:path');

const RAIZ_SERVIDOR = path.resolve(__dirname, '..', '..');

function leerRequerida(nombre) {
  const valor = process.env[nombre];
  if (valor === undefined || valor.trim() === '') {
    throw new Error(`La variable de entorno ${nombre} es obligatoria`);
  }
  return valor.trim();
}

function leerOpcional(nombre, valorPorDefecto) {
  const valor = process.env[nombre];
  return valor === undefined || valor.trim() === '' ? valorPorDefecto : valor.trim();
}

function leerEntero(nombre, valorPorDefecto) {
  const valor = Number.parseInt(leerOpcional(nombre, String(valorPorDefecto)), 10);
  if (Number.isNaN(valor)) {
    throw new Error(`La variable de entorno ${nombre} debe ser un número entero`);
  }
  return valor;
}

function cargarEntorno() {
  const esPrueba = leerOpcional('NODE_ENV', 'development') === 'test';

  return Object.freeze({
    esPrueba,
    http: Object.freeze({
      puerto: leerEntero('PORT', 3000),
      origenesPermitidos: leerOpcional('CORS_ORIGINS', '*')
    }),
    grpc: Object.freeze({
      puerto: leerEntero('GRPC_PORT', 50051),
      rutaProtos: path.resolve(leerOpcional('PROTOS_DIR', path.join(RAIZ_SERVIDOR, '..', 'protos')))
    }),
    jwt: Object.freeze({
      secreto: leerRequerida('JWT_SECRET'),
      expiracion: leerOpcional('JWT_EXPIRES_IN', '1h')
    }),
    baseDatos: Object.freeze({
      host: leerRequerida('DB_HOST'),
      puerto: leerEntero('DB_PORT', 1433),
      nombre: esPrueba ? leerRequerida('DB_TEST_NAME') : leerRequerida('DB_NAME'),
      usuario: leerRequerida('DB_USER'),
      contrasena: leerRequerida('DB_PASSWORD'),
      intentosConexion: leerEntero('DB_CONNECT_RETRIES', 20),
      esperaEntreIntentosMs: leerEntero('DB_CONNECT_RETRY_DELAY_MS', 5000)
    }),
    redis: Object.freeze({
      url: leerRequerida('REDIS_URL'),
      baseDatos: esPrueba ? 1 : 0
    }),
    multimedia: Object.freeze({
      directorio: path.resolve(leerOpcional('MEDIA_DIR', path.join(RAIZ_SERVIDOR, 'multimedia')))
    }),
    administrador: Object.freeze({
      correo: leerOpcional('ADMIN_EMAIL', ''),
      contrasena: leerOpcional('ADMIN_PASSWORD', ''),
      nombre: leerOpcional('ADMIN_NAME', 'Administrador')
    })
  });
}

module.exports = { cargarEntorno };
