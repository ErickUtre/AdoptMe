const { Op } = require('sequelize');

function crearAdopcionRepositorio({ Adopcion, Mascota, Ubicacion, Solicitud }) {
  const inclusionDetalle = [
    { model: Mascota, as: 'Mascota' },
    { model: Ubicacion, as: 'Ubicacion' }
  ];

  return Object.freeze({
    crear: async ({ mascota, ubicacion, publicadorId }, transaction) => {
      const mascotaCreada = await Mascota.create(mascota, { transaction });
      const ubicacionCreada = await Ubicacion.create(ubicacion, { transaction });
      return Adopcion.create({
        Estado: false,
        MascotaID: mascotaCreada.MascotaID,
        PublicadorID: publicadorId,
        UbicacionID: ubicacionCreada.UbicacionID
      }, { transaction });
    },

    buscarPorId: (adopcionId, transaction) => Adopcion.findByPk(adopcionId, { transaction }),

    buscarDetalle: (adopcionId, transaction) => Adopcion.findByPk(adopcionId, { include: inclusionDetalle, transaction }),

    buscarPorMascota: (mascotaId) => Adopcion.findOne({ where: { MascotaID: mascotaId } }),

    listarPorPublicador: (publicadorId) => Adopcion.findAll({
      where: { PublicadorID: publicadorId },
      include: inclusionDetalle,
      order: [['FechaSolicitud', 'DESC']]
    }),

    listarPorEstado: (estado) => Adopcion.findAll({
      where: estado === undefined ? {} : { Estado: estado },
      attributes: ['AdopcionID', 'Estado', 'FechaSolicitud', 'MascotaID', 'PublicadorID'],
      order: [['FechaSolicitud', 'ASC']]
    }),

    listarDisponiblesPorIds: (adopcionIds, publicadorExcluidoId) => Adopcion.findAll({
      where: {
        AdopcionID: { [Op.in]: adopcionIds },
        Estado: false,
        PublicadorID: { [Op.ne]: publicadorExcluidoId }
      },
      include: [{ model: Mascota, as: 'Mascota' }]
    }),

    listarDisponiblesConUbicacion: () => Adopcion.findAll({
      where: { Estado: false },
      attributes: ['AdopcionID'],
      include: [{ model: Ubicacion, as: 'Ubicacion', required: true }]
    }),

    actualizarEstado: (adopcionId, estado, transaction) => Adopcion.update(
      { Estado: estado },
      { where: { AdopcionID: adopcionId }, transaction }
    ),

    actualizarMascota: (mascotaId, cambios, transaction) => Mascota.update(cambios, { where: { MascotaID: mascotaId }, transaction }),

    actualizarUbicacion: (ubicacionId, cambios, transaction) => Ubicacion.update(cambios, { where: { UbicacionID: ubicacionId }, transaction }),

    eliminar: async (adopcion, transaction) => {
      await Solicitud.destroy({ where: { AdopcionID: adopcion.AdopcionID }, transaction });
      await Adopcion.destroy({ where: { AdopcionID: adopcion.AdopcionID }, transaction });
      await Mascota.destroy({ where: { MascotaID: adopcion.MascotaID }, transaction });
      await Ubicacion.destroy({ where: { UbicacionID: adopcion.UbicacionID }, transaction });
    }
  });
}

module.exports = { crearAdopcionRepositorio };
