module.exports = {
  collectCoverageFrom: ['src/**/*.js', '!src/index.js'],
  projects: [
    {
      displayName: 'unitarias',
      testEnvironment: 'node',
      testMatch: ['<rootDir>/pruebas/unitarias/**/*.prueba.js']
    },
    {
      displayName: 'integracion',
      testEnvironment: 'node',
      testMatch: ['<rootDir>/pruebas/integracion/**/*.prueba.js'],
      setupFiles: ['<rootDir>/pruebas/integracion/entorno.js'],
      setupFilesAfterEnv: ['<rootDir>/pruebas/integracion/tiempoEspera.js']
    }
  ]
};
