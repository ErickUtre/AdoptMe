function crearUsuarioRepositorio({ Usuario, Acceso, Ubicacion }) {
  const inclusionPerfil = [
    { model: Ubicacion, as: 'Ubicacion' },
    { model: Acceso, as: 'Acceso', attributes: ['Correo', 'EsAdmin'] }
  ];

  return Object.freeze({
    crearAcceso: (datos, transaction) => Acceso.create(datos, { transaction }),

    crearUsuario: (datos, transaction) => Usuario.create(datos, { transaction }),

    buscarAccesoPorCorreo: (correo, transaction) => Acceso.findOne({ where: { Correo: correo }, transaction }),

    buscarCuentaPorCorreo: (correo) => Acceso.findOne({
      where: { Correo: correo },
      include: [{ model: Usuario, as: 'Usuario', include: [{ model: Ubicacion, as: 'Ubicacion' }] }]
    }),

    buscarPorId: (usuarioId, transaction) => Usuario.findByPk(usuarioId, { transaction }),

    actualizar: (usuarioId, cambios, transaction) => Usuario.update(cambios, { where: { UsuarioID: usuarioId }, transaction }),

    actualizarAcceso: (accesoId, cambios, transaction) => Acceso.update(cambios, { where: { AccesoID: accesoId }, transaction }),

    buscarPerfil: (usuarioId) => Usuario.findByPk(usuarioId, { include: inclusionPerfil }),

    buscarConAcceso: (usuarioId, transaction) => Usuario.findByPk(usuarioId, {
      include: [{ model: Acceso, as: 'Acceso' }],
      transaction
    }),

    existe: async (usuarioId) => (await Usuario.count({ where: { UsuarioID: usuarioId } })) > 0,

    listarConUbicacion: () => Usuario.findAll({
      attributes: ['UsuarioID'],
      include: [{ model: Ubicacion, as: 'Ubicacion', required: true }]
    })
  });
}

module.exports = { crearUsuarioRepositorio };
