const { ErrorValidacion } = require('./errores');

const PATRON_CORREO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PATRON_TELEFONO = /^\d{10}$/;
const LONGITUD_MINIMA_CONTRASENA = 8;

function exigirIdentificador(valor, nombreCampo) {
  const numero = Number(valor);
  if (!Number.isInteger(numero) || numero <= 0) {
    throw new ErrorValidacion(`${nombreCampo} debe ser un entero positivo`);
  }
  return numero;
}

function exigirTexto(valor, nombreCampo, longitudMaxima) {
  if (typeof valor !== 'string' || valor.trim() === '') {
    throw new ErrorValidacion(`${nombreCampo} es obligatorio`);
  }
  const texto = valor.trim();
  if (longitudMaxima && texto.length > longitudMaxima) {
    throw new ErrorValidacion(`${nombreCampo} no debe exceder ${longitudMaxima} caracteres`);
  }
  return texto;
}

function textoOpcional(valor, nombreCampo, longitudMaxima) {
  if (valor === undefined || valor === null) {
    return undefined;
  }
  return exigirTexto(valor, nombreCampo, longitudMaxima);
}

function exigirCorreo(valor) {
  const correo = exigirTexto(valor, 'Correo', 100).toLowerCase();
  if (!PATRON_CORREO.test(correo)) {
    throw new ErrorValidacion('Correo no tiene un formato válido');
  }
  return correo;
}

function exigirContrasena(valor) {
  if (typeof valor !== 'string' || valor.length < LONGITUD_MINIMA_CONTRASENA) {
    throw new ErrorValidacion(`La contraseña debe tener al menos ${LONGITUD_MINIMA_CONTRASENA} caracteres`);
  }
  return valor;
}

function telefonoOpcional(valor) {
  if (valor === undefined || valor === null || valor === '') {
    return undefined;
  }
  const telefono = String(valor).trim();
  if (!PATRON_TELEFONO.test(telefono)) {
    throw new ErrorValidacion('Telefono debe contener 10 dígitos');
  }
  return telefono;
}

function aNumero(valor) {
  if (valor === null || valor === undefined || valor === '') {
    return Number.NaN;
  }
  return Number(valor);
}

function exigirCoordenadas(latitud, longitud) {
  const lat = aNumero(latitud);
  const lon = aNumero(longitud);
  const sonValidas = Number.isFinite(lat) && Number.isFinite(lon)
    && lat >= -90 && lat <= 90
    && lon >= -180 && lon <= 180;

  if (!sonValidas) {
    throw new ErrorValidacion('Coordenadas inválidas');
  }
  return { latitud: lat, longitud: lon };
}

function exigirUbicacion(ubicacion) {
  if (!ubicacion || typeof ubicacion !== 'object') {
    throw new ErrorValidacion('La ubicación es obligatoria');
  }
  const { latitud, longitud } = exigirCoordenadas(ubicacion.Latitud, ubicacion.Longitud);
  return {
    Latitud: latitud,
    Longitud: longitud,
    Ciudad: textoOpcional(ubicacion.Ciudad, 'Ciudad', 100) ?? null,
    Estado: textoOpcional(ubicacion.Estado, 'Estado', 100) ?? null,
    Pais: textoOpcional(ubicacion.Pais, 'Pais', 100) ?? null
  };
}

module.exports = {
  exigirIdentificador,
  exigirTexto,
  textoOpcional,
  exigirCorreo,
  exigirContrasena,
  telefonoOpcional,
  exigirCoordenadas,
  exigirUbicacion
};
