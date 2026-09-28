function crearNotificacionRepositorio({ Notificacion }) {
  return Object.freeze({
    crear: (datos) => Notificacion.create(datos),

    crearVarias: (lista) => Notificacion.bulkCreate(lista, { returning: true }),

    listarPorUsuario: (usuarioId) => Notificacion.findAll({
      where: { UsuarioID: usuarioId },
      order: [['FechaCreacion', 'DESC']]
    }),

    eliminar: (usuarioId, notificacionId) => Notificacion.destroy({
      where: { UsuarioID: usuarioId, NotificacionID: notificacionId }
    }),

    eliminarTodas: (usuarioId) => Notificacion.destroy({ where: { UsuarioID: usuarioId } })
  });
}

module.exports = { crearNotificacionRepositorio };
