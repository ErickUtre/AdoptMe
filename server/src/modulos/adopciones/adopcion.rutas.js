const { Router } = require('express');
const { ROLES } = require('../autenticacion/autenticacion.servicio');

function crearAdopcionRutas({ adopcionControlador, autenticacion }) {
  const rutas = Router();

  rutas.use(autenticacion.requerirSesion);
  rutas.get('/', autenticacion.requerirRol(ROLES.ADMINISTRADOR), adopcionControlador.listarParaReporte);
  rutas.post('/', adopcionControlador.registrar);
  rutas.get('/propias', adopcionControlador.listarPropias);
  rutas.get('/:id', adopcionControlador.obtenerDetalle);
  rutas.patch('/:id', adopcionControlador.modificar);
  rutas.delete('/:id', adopcionControlador.eliminar);

  return rutas;
}

module.exports = { crearAdopcionRutas };
