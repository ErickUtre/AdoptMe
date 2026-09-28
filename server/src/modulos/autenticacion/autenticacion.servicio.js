const { ErrorConflicto, ErrorNoAutenticado, ErrorNoEncontrado, ErrorValidacion } = require('../../compartido/errores');
const { exigirCorreo, exigirContrasena, exigirTexto } = require('../../compartido/validacion');
const { mapearPerfil } = require('../usuarios/usuario.mapeador');

const ROLES = Object.freeze({ ADMINISTRADOR: 'Admin', USUARIO: 'Usuario' });
const MENSAJE_CREDENCIALES = 'Correo o contraseña incorrectos';

function crearAutenticacionServicio({ usuarioRepositorio, contrasenas, tokens, transaccion }) {
  async function iniciarSesion(datos) {
    const correo = exigirTexto(datos?.Correo, 'Correo', 100).toLowerCase();
    const contrasena = exigirTexto(datos?.Contrasena, 'Contrasena');

    const acceso = await usuarioRepositorio.buscarCuentaPorCorreo(correo);
    if (!acceso?.Usuario || !(await contrasenas.coincide(contrasena, acceso.ContrasenaHash))) {
      throw new ErrorNoAutenticado(MENSAJE_CREDENCIALES);
    }

    if (contrasenas.requiereActualizacion(acceso.ContrasenaHash)) {
      await usuarioRepositorio.actualizarAcceso(acceso.AccesoID, { ContrasenaHash: await contrasenas.cifrar(contrasena) });
    }

    const rol = acceso.EsAdmin ? ROLES.ADMINISTRADOR : ROLES.USUARIO;
    return {
      token: tokens.emitir({ usuarioId: acceso.Usuario.UsuarioID, rol }),
      esAdmin: acceso.EsAdmin,
      usuario: mapearPerfil(acceso.Usuario, acceso)
    };
  }

  async function actualizarCredenciales(usuarioId, datos) {
    const correo = datos?.Correo === undefined ? undefined : exigirCorreo(datos.Correo);
    const contrasena = datos?.Contrasena === undefined ? undefined : exigirContrasena(datos.Contrasena);
    if (correo === undefined && contrasena === undefined) {
      throw new ErrorValidacion('No se proporcionaron datos para actualizar');
    }
    const contrasenaHash = contrasena === undefined ? undefined : await contrasenas.cifrar(contrasena);

    await transaccion(async (transaction) => {
      const usuario = await usuarioRepositorio.buscarConAcceso(usuarioId, transaction);
      if (!usuario?.Acceso) {
        throw new ErrorNoEncontrado('Acceso no encontrado');
      }
      if (correo !== undefined) {
        const existente = await usuarioRepositorio.buscarAccesoPorCorreo(correo, transaction);
        if (existente && existente.AccesoID !== usuario.AccesoID) {
          throw new ErrorConflicto('Ese correo ya está en uso');
        }
      }
      await usuarioRepositorio.actualizarAcceso(usuario.AccesoID, {
        ...(correo !== undefined && { Correo: correo }),
        ...(contrasenaHash !== undefined && { ContrasenaHash: contrasenaHash })
      }, transaction);
    });
  }

  return Object.freeze({ iniciarSesion, actualizarCredenciales });
}

module.exports = { crearAutenticacionServicio, ROLES };
