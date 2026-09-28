const { ErrorValidacion } = require('../../compartido/errores');
const { exigirTexto, textoOpcional } = require('../../compartido/validacion');

const SEXOS_VALIDOS = Object.freeze(['Macho', 'Hembra']);

const CAMPOS = Object.freeze({
  Nombre: 45,
  Especie: 50,
  Raza: 100,
  Edad: 100,
  Tamaño: 10
});

function validarSexo(valor) {
  if (!SEXOS_VALIDOS.includes(valor)) {
    throw new ErrorValidacion(`Sexo debe ser uno de: ${SEXOS_VALIDOS.join(', ')}`);
  }
  return valor;
}

function exigirMascota(datos) {
  if (!datos || typeof datos !== 'object') {
    throw new ErrorValidacion('Los datos de la mascota son obligatorios');
  }
  const mascota = Object.fromEntries(
    Object.entries(CAMPOS).map(([campo, longitud]) => [campo, exigirTexto(datos[campo], campo, longitud)])
  );
  return {
    ...mascota,
    Sexo: validarSexo(exigirTexto(datos.Sexo, 'Sexo')),
    Descripcion: textoOpcional(datos.Descripcion, 'Descripcion', 2000) ?? null
  };
}

function cambiosMascota(datos) {
  if (!datos || typeof datos !== 'object') {
    return {};
  }
  const cambios = {};
  for (const [campo, longitud] of Object.entries(CAMPOS)) {
    const valor = textoOpcional(datos[campo], campo, longitud);
    if (valor !== undefined) {
      cambios[campo] = valor;
    }
  }
  if (datos.Sexo !== undefined) {
    cambios.Sexo = validarSexo(exigirTexto(datos.Sexo, 'Sexo'));
  }
  if (datos.Descripcion !== undefined) {
    cambios.Descripcion = textoOpcional(datos.Descripcion, 'Descripcion', 2000) ?? null;
  }
  return cambios;
}

module.exports = { exigirMascota, cambiosMascota };
