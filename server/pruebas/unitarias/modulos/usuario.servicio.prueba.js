const { crearUsuarioServicio } = require('../../../src/modulos/usuarios/usuario.servicio');
const { ErrorConflicto, ErrorValidacion } = require('../../../src/compartido/errores');
const { registroSilencioso, transaccionDirecta, crearIndiceGeograficoFalso } = require('../apoyo');

function crearDependencias({ correoExistente = false } = {}) {
  const usuarioRepositorio = {
    buscarAccesoPorCorreo: jest.fn().mockResolvedValue(correoExistente ? { AccesoID: 1 } : null),
    crearAcceso: jest.fn().mockResolvedValue({ AccesoID: 10 }),
    crearUsuario: jest.fn().mockResolvedValue({ UsuarioID: 20 }),
    buscarPorId: jest.fn().mockResolvedValue({ UsuarioID: 20, UbicacionID: 5 }),
    actualizar: jest.fn().mockResolvedValue(),
    buscarPerfil: jest.fn().mockResolvedValue({
      UsuarioID: 20, Nombre: 'Ana', Telefono: null, Ubicacion: null, Acceso: { Correo: 'ana@correo.com', EsAdmin: false }
    })
  };
  const ubicacionRepositorio = {
    crear: jest.fn().mockResolvedValue({ UbicacionID: 30, Latitud: 19.5, Longitud: -96.9 }),
    eliminar: jest.fn().mockResolvedValue()
  };
  const indiceGeografico = crearIndiceGeograficoFalso();
  const contrasenas = { cifrar: jest.fn().mockResolvedValue('hash') };
  const servicio = crearUsuarioServicio({
    usuarioRepositorio, ubicacionRepositorio, indiceGeografico, contrasenas, transaccion: transaccionDirecta, registro: registroSilencioso
  });
  return { servicio, usuarioRepositorio, ubicacionRepositorio, indiceGeografico, contrasenas };
}

const registroValido = {
  Nombre: 'Ana',
  Telefono: '2281234567',
  Ubicacion: { Latitud: 19.5, Longitud: -96.9 },
  Acceso: { Correo: 'Ana@Correo.com', Contrasena: 'Segura123', EsAdmin: true }
};

describe('usuarioServicio.registrar', () => {
  test('crea la cuenta con la contraseña cifrada, nunca como administrador, e indexa la ubicación', async () => {
    const { servicio, usuarioRepositorio, indiceGeografico, contrasenas } = crearDependencias();

    await expect(servicio.registrar(registroValido)).resolves.toEqual({ UsuarioID: 20 });

    expect(contrasenas.cifrar).toHaveBeenCalledWith('Segura123');
    expect(usuarioRepositorio.crearAcceso).toHaveBeenCalledWith(
      { Correo: 'ana@correo.com', ContrasenaHash: 'hash', EsAdmin: false }, 'transaccion'
    );
    expect(indiceGeografico.registrarUsuario).toHaveBeenCalledWith(20, { latitud: 19.5, longitud: -96.9 });
  });

  test('rechaza correos ya registrados', async () => {
    const { servicio } = crearDependencias({ correoExistente: true });
    await expect(servicio.registrar(registroValido)).rejects.toBeInstanceOf(ErrorConflicto);
  });

  test('rechaza coordenadas inválidas antes de tocar la base de datos', async () => {
    const { servicio, usuarioRepositorio } = crearDependencias();
    await expect(servicio.registrar({ ...registroValido, Ubicacion: { Latitud: 200, Longitud: 0 } }))
      .rejects.toBeInstanceOf(ErrorValidacion);
    expect(usuarioRepositorio.crearAcceso).not.toHaveBeenCalled();
  });
});

describe('usuarioServicio.actualizarUbicacion', () => {
  test('reemplaza la ubicación anterior y actualiza el índice', async () => {
    const { servicio, usuarioRepositorio, ubicacionRepositorio, indiceGeografico } = crearDependencias();

    await servicio.actualizarUbicacion(20, { Latitud: 19.5, Longitud: -96.9, Ciudad: 'Xalapa' });

    expect(usuarioRepositorio.actualizar).toHaveBeenCalledWith(20, { UbicacionID: 30 }, 'transaccion');
    expect(ubicacionRepositorio.eliminar).toHaveBeenCalledWith(5, 'transaccion');
    expect(indiceGeografico.registrarUsuario).toHaveBeenCalledWith(20, { latitud: 19.5, longitud: -96.9 });
  });
});
