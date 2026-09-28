const { crearAutenticacionServicio } = require('../../../src/modulos/autenticacion/autenticacion.servicio');
const { ErrorNoAutenticado, ErrorConflicto } = require('../../../src/compartido/errores');
const { transaccionDirecta } = require('../apoyo');

function crearCuenta({ hash = 'hash-vigente', esAdmin = false } = {}) {
  return {
    AccesoID: 3,
    Correo: 'ana@correo.com',
    ContrasenaHash: hash,
    EsAdmin: esAdmin,
    Usuario: { UsuarioID: 9, Nombre: 'Ana', Telefono: null, FechaRegistro: null, Ubicacion: null }
  };
}

function crearDependencias({ cuenta = crearCuenta(), coincide = true, requiereActualizacion = false } = {}) {
  const usuarioRepositorio = {
    buscarCuentaPorCorreo: jest.fn().mockResolvedValue(cuenta),
    actualizarAcceso: jest.fn().mockResolvedValue(),
    buscarConAcceso: jest.fn().mockResolvedValue({ AccesoID: 3, Acceso: { AccesoID: 3 } }),
    buscarAccesoPorCorreo: jest.fn().mockResolvedValue({ AccesoID: 99 })
  };
  const contrasenas = {
    coincide: jest.fn().mockResolvedValue(coincide),
    requiereActualizacion: jest.fn().mockReturnValue(requiereActualizacion),
    cifrar: jest.fn().mockResolvedValue('hash-nuevo')
  };
  const tokens = { emitir: jest.fn().mockReturnValue('token') };
  const servicio = crearAutenticacionServicio({ usuarioRepositorio, contrasenas, tokens, transaccion: transaccionDirecta });
  return { servicio, usuarioRepositorio, contrasenas, tokens };
}

describe('autenticacionServicio.iniciarSesion', () => {
  test('devuelve el token y el perfil sin exponer el hash', async () => {
    const { servicio, tokens } = crearDependencias({ cuenta: crearCuenta({ esAdmin: true }) });

    const sesion = await servicio.iniciarSesion({ Correo: 'ANA@correo.com', Contrasena: 'Segura123' });

    expect(tokens.emitir).toHaveBeenCalledWith({ usuarioId: 9, rol: 'Admin' });
    expect(sesion).toMatchObject({ token: 'token', esAdmin: true, usuario: { UsuarioID: 9, Acceso: { Correo: 'ana@correo.com', EsAdmin: true } } });
    expect(JSON.stringify(sesion)).not.toContain('hash-vigente');
  });

  test('rechaza credenciales incorrectas', async () => {
    const { servicio } = crearDependencias({ coincide: false });
    await expect(servicio.iniciarSesion({ Correo: 'ana@correo.com', Contrasena: 'x' })).rejects.toBeInstanceOf(ErrorNoAutenticado);
  });

  test('migra a bcrypt los hashes heredados tras un inicio de sesión válido', async () => {
    const { servicio, usuarioRepositorio } = crearDependencias({ requiereActualizacion: true });
    await servicio.iniciarSesion({ Correo: 'ana@correo.com', Contrasena: 'Segura123' });
    expect(usuarioRepositorio.actualizarAcceso).toHaveBeenCalledWith(3, { ContrasenaHash: 'hash-nuevo' });
  });
});

describe('autenticacionServicio.actualizarCredenciales', () => {
  test('impide usar un correo que pertenece a otra cuenta', async () => {
    const { servicio } = crearDependencias();
    await expect(servicio.actualizarCredenciales(9, { Correo: 'otro@correo.com' })).rejects.toBeInstanceOf(ErrorConflicto);
  });
});
