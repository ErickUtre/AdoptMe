const { DataTypes } = require('sequelize');

function definirNotificacion(sequelize) {
  return sequelize.define('Notificacion', {
    NotificacionID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    UsuarioID: { type: DataTypes.INTEGER, allowNull: false },
    Titulo: { type: DataTypes.STRING(100), allowNull: false },
    Mensaje: { type: DataTypes.TEXT, allowNull: false },
    Tipo: { type: DataTypes.STRING(50), allowNull: false },
    FechaCreacion: { type: DataTypes.DATE, allowNull: true },
    Leida: { type: DataTypes.BOOLEAN, allowNull: false, defaultValue: false },
    ReferenciaID: { type: DataTypes.INTEGER, allowNull: true },
    ReferenciaTipo: { type: DataTypes.STRING(50), allowNull: true }
  }, {
    tableName: 'Notificacion',
    timestamps: false
  });
}

module.exports = { definirNotificacion };
