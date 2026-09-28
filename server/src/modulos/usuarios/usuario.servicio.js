const { ErrorConflicto, ErrorNoEncontrado, ErrorValidacion } = require('../../compartido/errores');
const {
  exigirTexto, exigirCorreo, exigirContrasena, telefonoOpcional, textoOpcional, exigirUbicacion
} = require('../../compartido/validacion');
const { mapearPerfil, mapearUbicacion } = require('./usuario.mapeador');

function crearUsuarioServicio({ usuarioRepositorio, ubicacionRepositorio, indiceGeografico, contrasenas, transaccion, registro }) {
  async function exigirUsuario(usuarioId, transaction) {
    const usuario = await usuarioRepositorio.buscarPorId(usuarioId, transaction);
    if (!usuario) {
      throw new ErrorNoEncontrado('Usuario no encontrado');
    }
    return usuario;
  }

  async function indexar(usuarioId, ubicacion) {
    try {
      await indiceGeografico.registrarUsuario(usuarioId, { latitud: ubicacion.Latitud, longitud: ubicacion.Longitud });
    } catch (error) {
      registro.error('No se pudo indexar la ubicación del usuario', { usuarioId, causa: error.message });
    }
  }

  async function registrar(datos) {
    const nombre = exigirTexto(datos?.Nombre, 'Nombre', 100);
    const telefono = telefonoOpcional(datos?.Telefono);
    const correo = exigirCorreo(datos?.Acceso?.Correo);
    const contrasena = exigirContrasena(datos?.Acceso?.Contrasena);
    const ubicacion = datos?.Ubicacion ? exigirUbicacion(datos.Ubicacion) : null;
    const contrasenaHash = await contrasenas.cifrar(contrasena);

    const usuario = await transaccion(async (transaction) => {
      if (await usuarioRepositorio.buscarAccesoPorCorreo(correo, transaction)) {
        throw new ErrorConflicto('El correo ya está registrado');
      }
      const acceso = await usuarioRepositorio.crearAcceso({ Correo: correo, ContrasenaHash: contrasenaHash, EsAdmin: false }, transaction);
      const ubicacionCreada = ubicacion ? await ubicacionRepositorio.crear(ubicacion, transaction) : null;
      return usuarioRepositorio.crearUsuario({
        Nombre: nombre,
        Telefono: telefono ?? null,
        UbicacionID: ubicacionCreada?.UbicacionID ?? null,
        AccesoID: acceso.AccesoID
      }, transaction);
    });

    if (ubicacion) {
      await indexar(usuario.UsuarioID, ubicacion);
    }

    return { UsuarioID: usuario.UsuarioID };
  }

  async function obtenerPerfil(usuarioId) {
    const usuario = await usuarioRepositorio.buscarPerfil(usuarioId);
    if (!usuario) {
      throw new ErrorNoEncontrado('Usuario no encontrado');
    }
    return mapearPerfil(usuario);
  }

  async function obtenerResumen(usuarioId) {
    const usuario = await exigirUsuario(usuarioId);
    return { UsuarioID: usuario.UsuarioID, Nombre: usuario.Nombre };
  }

  async function actualizarPerfil(usuarioId, datos) {
    const nombre = textoOpcional(datos?.Nombre, 'Nombre', 100);
    const telefono = telefonoOpcional(datos?.Telefono);
    if (nombre === undefined && telefono === undefined) {
      throw new ErrorValidacion('No se proporcionaron datos para actualizar');
    }

    await exigirUsuario(usuarioId);
    await usuarioRepositorio.actualizar(usuarioId, {
      ...(nombre !== undefined && { Nombre: nombre }),
      ...(telefono !== undefined && { Telefono: telefono })
    });
    return obtenerPerfil(usuarioId);
  }

  async function actualizarUbicacion(usuarioId, datos) {
    const ubicacion = exigirUbicacion(datos);

    const nuevaUbicacion = await transaccion(async (transaction) => {
      const usuario = await exigirUsuario(usuarioId, transaction);
      const ubicacionAnteriorId = usuario.UbicacionID;
      const creada = await ubicacionRepositorio.crear(ubicacion, transaction);
      await usuarioRepositorio.actualizar(usuarioId, { UbicacionID: creada.UbicacionID }, transaction);
      if (ubicacionAnteriorId) {
        await ubicacionRepositorio.eliminar(ubicacionAnteriorId, transaction);
      }
      return creada;
    });

    await indexar(usuarioId, ubicacion);
    return mapearUbicacion(nuevaUbicacion);
  }

  return Object.freeze({ registrar, obtenerPerfil, obtenerResumen, actualizarPerfil, actualizarUbicacion });
}

module.exports = { crearUsuarioServicio };
