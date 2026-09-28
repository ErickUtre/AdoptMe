const { DataTypes } = require('sequelize');

function definirMascota(sequelize) {
  return sequelize.define('Mascota', {
    MascotaID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    Nombre: { type: DataTypes.STRING(45), allowNull: false },
    Especie: { type: DataTypes.STRING(50), allowNull: false },
    Raza: { type: DataTypes.STRING(100), allowNull: false },
    Edad: { type: DataTypes.STRING(100), allowNull: false },
    Sexo: { type: DataTypes.STRING(10), allowNull: false },
    Tamaño: { type: DataTypes.STRING(10), allowNull: false },
    Descripcion: { type: DataTypes.TEXT, allowNull: true }
  }, {
    tableName: 'Mascota',
    timestamps: false
  });
}

module.exports = { definirMascota };
