function crearUbicacionRepositorio({ Ubicacion }) {
  return Object.freeze({
    crear: (datos, transaction) => Ubicacion.create(datos, { transaction }),
    eliminar: (ubicacionId, transaction) => Ubicacion.destroy({ where: { UbicacionID: ubicacionId }, transaction }),
    actualizar: (ubicacionId, cambios, transaction) => Ubicacion.update(cambios, { where: { UbicacionID: ubicacionId }, transaction })
  });
}

module.exports = { crearUbicacionRepositorio };
