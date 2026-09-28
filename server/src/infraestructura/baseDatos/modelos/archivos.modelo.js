const { DataTypes } = require('sequelize');

function definirFotoUsuario(sequelize) {
  return sequelize.define('FotoUsuario', {
    FotoUsuarioID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    UsuarioID: { type: DataTypes.INTEGER, allowNull: false, unique: true },
    UrlFoto: { type: DataTypes.STRING(255), allowNull: false }
  }, {
    tableName: 'FotoUsuario',
    timestamps: false
  });
}

function definirFotoMascota(sequelize) {
  return sequelize.define('FotoMascota', {
    FotoID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    MascotaID: { type: DataTypes.INTEGER, allowNull: false },
    UrlFoto: { type: DataTypes.STRING(255), allowNull: false }
  }, {
    tableName: 'FotoMascota',
    timestamps: false
  });
}

function definirVideoMascota(sequelize) {
  return sequelize.define('VideoMascota', {
    VideoID: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
    MascotaID: { type: DataTypes.INTEGER, allowNull: false },
    UrlVideo: { type: DataTypes.STRING(255), allowNull: false }
  }, {
    tableName: 'VideoMascota',
    timestamps: false
  });
}

module.exports = { definirFotoUsuario, definirFotoMascota, definirVideoMascota };
