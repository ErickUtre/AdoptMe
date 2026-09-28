const { Router } = require('express');

function crearNotificacionRutas({ notificacionControlador, autenticacion }) {
  const rutas = Router();

  rutas.use(autenticacion.requerirSesion);
  rutas.get('/', notificacionControlador.listar);
  rutas.delete('/', notificacionControlador.eliminarTodas);
  rutas.delete('/:id', notificacionControlador.eliminar);

  return rutas;
}

module.exports = { crearNotificacionRutas };
