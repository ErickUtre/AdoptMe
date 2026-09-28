const crypto = require('node:crypto');
const { crearServicioContrasenas } = require('../../../src/infraestructura/seguridad/contrasenas');
const { crearServicioTokens } = require('../../../src/infraestructura/seguridad/tokens');
const { ErrorNoAutenticado } = require('../../../src/compartido/errores');

describe('servicio de contraseñas', () => {
  const contrasenas = crearServicioContrasenas({ rondas: 4 });

  test('cifra con bcrypt y verifica la contraseña', async () => {
    const hash = await contrasenas.cifrar('Segura123');
    expect(hash).not.toBe('Segura123');
    await expect(contrasenas.coincide('Segura123', hash)).resolves.toBe(true);
    await expect(contrasenas.coincide('Otra1234', hash)).resolves.toBe(false);
    expect(contrasenas.requiereActualizacion(hash)).toBe(false);
  });

  test('reconoce los hashes SHA-512 heredados y pide actualizarlos', async () => {
    const heredado = crypto.createHash('sha512').update('Segura123').digest('hex');
    await expect(contrasenas.coincide('Segura123', heredado)).resolves.toBe(true);
    await expect(contrasenas.coincide('Incorrecta1', heredado)).resolves.toBe(false);
    expect(contrasenas.requiereActualizacion(heredado)).toBe(true);
  });
});

describe('servicio de tokens', () => {
  const tokens = crearServicioTokens({ secreto: 'secreto-de-prueba', expiracion: '1h' });

  test('emite y verifica una sesión', () => {
    const token = tokens.emitir({ usuarioId: 7, rol: 'Usuario' });
    expect(tokens.verificarEncabezado(`Bearer ${token}`)).toEqual({ usuarioId: 7, rol: 'Usuario' });
  });

  test('rechaza encabezados ausentes o tokens alterados', () => {
    expect(() => tokens.verificarEncabezado(undefined)).toThrow(ErrorNoAutenticado);
    expect(() => tokens.verificarEncabezado('Bearer token.invalido.123')).toThrow(ErrorNoAutenticado);
  });
});
