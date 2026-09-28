const { DataTypes } = require('sequelize');

function definirAcceso(sequelize) {
  return sequelize.define('Acceso', {
    AccesoID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    Correo: { type: DataTypes.STRING(100), allowNull: false, unique: true, validate: { isEmail: true } },
    ContrasenaHash: { type: DataTypes.STRING(255), allowNull: false },
    EsAdmin: { type: DataTypes.BOOLEAN, allowNull: false, defaultValue: false }
  }, {
    tableName: 'Acceso',
    timestamps: false
  });
}

module.exports = { definirAcceso };
