const { Router } = require('express');

function crearAutenticacionRutas({ autenticacionControlador, autenticacion }) {
  const rutas = Router();

  rutas.post('/iniciar-sesion', autenticacionControlador.iniciarSesion);
  rutas.patch('/', autenticacion.requerirSesion, autenticacionControlador.actualizarCredenciales);

  return rutas;
}

module.exports = { crearAutenticacionRutas };
