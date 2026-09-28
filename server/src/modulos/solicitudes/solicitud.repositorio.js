function crearSolicitudRepositorio({ Solicitud, Usuario }) {
  return Object.freeze({
    crear: (datos) => Solicitud.create(datos),

    buscarPorId: (solicitudId, transaction) => Solicitud.findByPk(solicitudId, { transaction }),

    existe: async (adoptanteId, adopcionId) => (await Solicitud.count({
      where: { AdoptanteID: adoptanteId, AdopcionID: adopcionId }
    })) > 0,

    listarPorAdopcion: (adopcionId) => Solicitud.findAll({
      where: { AdopcionID: adopcionId },
      include: [{ model: Usuario, as: 'Adoptante', attributes: ['Nombre'] }],
      order: [['SolicitudID', 'ASC']]
    }),

    eliminar: (solicitudId, transaction) => Solicitud.destroy({ where: { SolicitudID: solicitudId }, transaction }),

    eliminarPorAdopcion: (adopcionId, transaction) => Solicitud.destroy({ where: { AdopcionID: adopcionId }, transaction })
  });
}

module.exports = { crearSolicitudRepositorio };
