const {
  exigirIdentificador, exigirCorreo, exigirContrasena, telefonoOpcional, exigirCoordenadas, exigirUbicacion
} = require('../../../src/compartido/validacion');
const { ErrorValidacion } = require('../../../src/compartido/errores');

describe('validacion', () => {
  test('acepta identificadores enteros positivos y rechaza el resto', () => {
    expect(exigirIdentificador('15', 'id')).toBe(15);
    for (const invalido of ['0', '-3', 'abc', '1.5', undefined]) {
      expect(() => exigirIdentificador(invalido, 'id')).toThrow(ErrorValidacion);
    }
  });

  test('normaliza el correo a minúsculas y valida su formato', () => {
    expect(exigirCorreo('  Persona@Correo.COM ')).toBe('persona@correo.com');
    expect(() => exigirCorreo('sin-arroba')).toThrow(ErrorValidacion);
  });

  test('exige una longitud mínima de contraseña', () => {
    expect(exigirContrasena('Segura123')).toBe('Segura123');
    expect(() => exigirContrasena('corta')).toThrow(ErrorValidacion);
  });

  test('valida teléfonos opcionales de 10 dígitos', () => {
    expect(telefonoOpcional(undefined)).toBeUndefined();
    expect(telefonoOpcional('2281234567')).toBe('2281234567');
    expect(() => telefonoOpcional('123')).toThrow(ErrorValidacion);
  });

  test('rechaza coordenadas ausentes o fuera de rango', () => {
    expect(exigirCoordenadas('19.5', '-96.9')).toEqual({ latitud: 19.5, longitud: -96.9 });
    expect(() => exigirCoordenadas(null, 10)).toThrow(ErrorValidacion);
    expect(() => exigirCoordenadas('', 10)).toThrow(ErrorValidacion);
    expect(() => exigirCoordenadas(190, -196)).toThrow(ErrorValidacion);
  });

  test('construye una ubicación completa', () => {
    expect(exigirUbicacion({ Latitud: 19.54, Longitud: -96.93, Ciudad: 'Xalapa' })).toEqual({
      Latitud: 19.54, Longitud: -96.93, Ciudad: 'Xalapa', Estado: null, Pais: null
    });
  });
});
