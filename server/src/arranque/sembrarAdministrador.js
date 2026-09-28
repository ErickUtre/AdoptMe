async function sembrarAdministrador({ configuracion, usuarioRepositorio, contrasenas, transaccion, registro }) {
  const { correo, contrasena, nombre } = configuracion;
  if (!correo || !contrasena) {
    registro.advertencia('No se configuró un administrador inicial (ADMIN_EMAIL y ADMIN_PASSWORD)');
    return;
  }

  const correoNormalizado = correo.toLowerCase();
  if (await usuarioRepositorio.buscarAccesoPorCorreo(correoNormalizado)) {
    return;
  }

  const contrasenaHash = await contrasenas.cifrar(contrasena);
  await transaccion(async (transaction) => {
    const acceso = await usuarioRepositorio.crearAcceso(
      { Correo: correoNormalizado, ContrasenaHash: contrasenaHash, EsAdmin: true },
      transaction
    );
    await usuarioRepositorio.crearUsuario({ Nombre: nombre, AccesoID: acceso.AccesoID }, transaction);
  });
  registro.info('Administrador inicial creado', { correo: correoNormalizado });
}

module.exports = { sembrarAdministrador };
