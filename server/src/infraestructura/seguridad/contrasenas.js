const crypto = require('node:crypto');
const bcrypt = require('bcryptjs');

const RONDAS_SAL = 12;
const PATRON_BCRYPT = /^\$2[aby]\$\d{2}\$/;

function sha512Hex(texto) {
  return crypto.createHash('sha512').update(texto, 'utf8').digest('hex');
}

function igualesEnTiempoConstante(a, b) {
  const bufferA = Buffer.from(a);
  const bufferB = Buffer.from(b);
  return bufferA.length === bufferB.length && crypto.timingSafeEqual(bufferA, bufferB);
}

function crearServicioContrasenas({ rondas = RONDAS_SAL } = {}) {
  const esHashVigente = (hash) => PATRON_BCRYPT.test(hash);

  async function coincide(contrasena, hash) {
    if (esHashVigente(hash)) {
      return bcrypt.compare(contrasena, hash);
    }
    return igualesEnTiempoConstante(sha512Hex(contrasena), hash.toLowerCase());
  }

  return Object.freeze({
    cifrar: (contrasena) => bcrypt.hash(contrasena, rondas),
    coincide,
    requiereActualizacion: (hash) => !esHashVigente(hash)
  });
}

module.exports = { crearServicioContrasenas };
