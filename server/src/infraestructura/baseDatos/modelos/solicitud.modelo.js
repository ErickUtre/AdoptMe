const { DataTypes } = require('sequelize');

function definirSolicitud(sequelize) {
  return sequelize.define('Solicitud', {
    SolicitudID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    AdoptanteID: { type: DataTypes.INTEGER, allowNull: false },
    AdopcionID: { type: DataTypes.INTEGER, allowNull: false }
  }, {
    tableName: 'Solicitud',
    timestamps: false
  });
}

module.exports = { definirSolicitud };
