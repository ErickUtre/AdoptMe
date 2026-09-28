const { Sequelize } = require('sequelize');
const { definirModelos } = require('./modelos');
const { reintentar } = require('../../compartido/reintentar');

function crearSequelize(configuracion) {
  return new Sequelize(configuracion.nombre, configuracion.usuario, configuracion.contrasena, {
    host: configuracion.host,
    port: configuracion.puerto,
    dialect: 'mssql',
    dialectOptions: {
      options: {
        encrypt: true,
        trustServerCertificate: true
      }
    },
    pool: { max: 10, min: 0, idle: 10000 },
    logging: false
  });
}

async function conectarBaseDatos(configuracion, registro) {
  const sequelize = crearSequelize(configuracion);

  await reintentar(() => sequelize.authenticate(), {
    intentos: configuracion.intentosConexion,
    esperaMs: configuracion.esperaEntreIntentosMs,
    alFallar: (error, intento) => registro.advertencia('Base de datos no disponible', {
      intento,
      de: configuracion.intentosConexion,
      causa: error.message
    })
  });

  registro.info('Conexión a la base de datos establecida');

  return {
    sequelize,
    modelos: definirModelos(sequelize),
    transaccion: (trabajo) => sequelize.transaction(trabajo),
    cerrar: () => sequelize.close()
  };
}

module.exports = { conectarBaseDatos };
