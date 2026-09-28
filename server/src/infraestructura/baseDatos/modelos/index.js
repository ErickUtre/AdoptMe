const { definirAcceso } = require('./acceso.modelo');
const { definirUbicacion } = require('./ubicacion.modelo');
const { definirUsuario } = require('./usuario.modelo');
const { definirMascota } = require('./mascota.modelo');
const { definirAdopcion } = require('./adopcion.modelo');
const { definirSolicitud } = require('./solicitud.modelo');
const { definirChat } = require('./chat.modelo');
const { definirNotificacion } = require('./notificacion.modelo');
const { definirFotoUsuario, definirFotoMascota, definirVideoMascota } = require('./archivos.modelo');

function asociar(modelos) {
  const {
    Acceso, Ubicacion, Usuario, Mascota, Adopcion, Solicitud,
    Chat, Notificacion, FotoUsuario, FotoMascota, VideoMascota
  } = modelos;

  Acceso.hasOne(Usuario, { foreignKey: 'AccesoID', as: 'Usuario' });
  Usuario.belongsTo(Acceso, { foreignKey: 'AccesoID', as: 'Acceso' });

  Usuario.belongsTo(Ubicacion, { foreignKey: 'UbicacionID', as: 'Ubicacion' });
  Usuario.hasOne(FotoUsuario, { foreignKey: 'UsuarioID', as: 'Foto' });

  Mascota.hasMany(FotoMascota, { foreignKey: 'MascotaID', as: 'Fotos' });
  Mascota.hasMany(VideoMascota, { foreignKey: 'MascotaID', as: 'Videos' });

  Adopcion.belongsTo(Mascota, { foreignKey: 'MascotaID', as: 'Mascota' });
  Adopcion.belongsTo(Usuario, { foreignKey: 'PublicadorID', as: 'Publicador' });
  Adopcion.belongsTo(Ubicacion, { foreignKey: 'UbicacionID', as: 'Ubicacion' });
  Adopcion.hasMany(Solicitud, { foreignKey: 'AdopcionID', as: 'Solicitudes' });

  Solicitud.belongsTo(Usuario, { foreignKey: 'AdoptanteID', as: 'Adoptante' });
  Solicitud.belongsTo(Adopcion, { foreignKey: 'AdopcionID', as: 'Adopcion' });

  Chat.belongsTo(Usuario, { foreignKey: 'RemitenteID', as: 'Remitente' });
  Chat.belongsTo(Usuario, { foreignKey: 'DestinatarioID', as: 'Destinatario' });

  Notificacion.belongsTo(Usuario, { foreignKey: 'UsuarioID', as: 'Usuario' });
}

function definirModelos(sequelize) {
  const modelos = {
    Acceso: definirAcceso(sequelize),
    Ubicacion: definirUbicacion(sequelize),
    Usuario: definirUsuario(sequelize),
    Mascota: definirMascota(sequelize),
    Adopcion: definirAdopcion(sequelize),
    Solicitud: definirSolicitud(sequelize),
    Chat: definirChat(sequelize),
    Notificacion: definirNotificacion(sequelize),
    FotoUsuario: definirFotoUsuario(sequelize),
    FotoMascota: definirFotoMascota(sequelize),
    VideoMascota: definirVideoMascota(sequelize)
  };
  asociar(modelos);
  return Object.freeze(modelos);
}

module.exports = { definirModelos };
