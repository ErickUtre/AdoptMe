const { DataTypes } = require('sequelize');

function definirUsuario(sequelize) {
  return sequelize.define('Usuario', {
    UsuarioID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    Nombre: { type: DataTypes.STRING(100), allowNull: false },
    FechaRegistro: { type: DataTypes.DATE, allowNull: true },
    Telefono: { type: DataTypes.STRING(15), allowNull: true },
    UbicacionID: { type: DataTypes.INTEGER, allowNull: true },
    AccesoID: { type: DataTypes.INTEGER, allowNull: false }
  }, {
    tableName: 'Usuario',
    timestamps: false
  });
}

module.exports = { definirUsuario };
