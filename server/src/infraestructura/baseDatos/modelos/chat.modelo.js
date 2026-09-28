const { DataTypes } = require('sequelize');

function definirChat(sequelize) {
  return sequelize.define('Chat', {
    ChatID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    RemitenteID: { type: DataTypes.INTEGER, allowNull: false },
    DestinatarioID: { type: DataTypes.INTEGER, allowNull: false },
    Contenido: { type: DataTypes.TEXT, allowNull: false },
    FechaEnvio: { type: DataTypes.DATE, allowNull: true }
  }, {
    tableName: 'Chat',
    timestamps: false
  });
}

module.exports = { definirChat };
