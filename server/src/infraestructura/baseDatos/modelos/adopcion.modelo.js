const { DataTypes } = require('sequelize');

function definirAdopcion(sequelize) {
  return sequelize.define('Adopcion', {
    AdopcionID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    FechaSolicitud: { type: DataTypes.DATE, allowNull: true },
    Estado: { type: DataTypes.BOOLEAN, allowNull: false, defaultValue: false },
    MascotaID: { type: DataTypes.INTEGER, allowNull: false },
    PublicadorID: { type: DataTypes.INTEGER, allowNull: false },
    UbicacionID: { type: DataTypes.INTEGER, allowNull: false }
  }, {
    tableName: 'Adopcion',
    timestamps: false
  });
}

module.exports = { definirAdopcion };
