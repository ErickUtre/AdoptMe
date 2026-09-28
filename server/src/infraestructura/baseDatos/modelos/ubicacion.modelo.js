const { DataTypes } = require('sequelize');

function decimalComoNumero(campo) {
  return function obtener() {
    const valor = this.getDataValue(campo);
    return valor === null || valor === undefined ? null : Number(valor);
  };
}

function definirUbicacion(sequelize) {
  return sequelize.define('Ubicacion', {
    UbicacionID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    Longitud: { type: DataTypes.DECIMAL(19, 13), allowNull: true, get: decimalComoNumero('Longitud') },
    Latitud: { type: DataTypes.DECIMAL(19, 13), allowNull: true, get: decimalComoNumero('Latitud') },
    Ciudad: { type: DataTypes.STRING(100), allowNull: true },
    Estado: { type: DataTypes.STRING(100), allowNull: true },
    Pais: { type: DataTypes.STRING(100), allowNull: true }
  }, {
    tableName: 'Ubicacion',
    timestamps: false
  });
}

module.exports = { definirUbicacion };
