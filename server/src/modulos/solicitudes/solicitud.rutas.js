const { Router } = require('express');

function crearSolicitudRutas({ solicitudControlador, autenticacion }) {
  const rutas = Router({ mergeParams: true });

  rutas.use(autenticacion.requerirSesion);
  rutas.get('/', solicitudControlador.listar);
  rutas.post('/', solicitudControlador.registrar);
  rutas.post('/:solicitudId/aceptacion', solicitudControlador.aceptar);
  rutas.delete('/:solicitudId', solicitudControlador.rechazar);

  return rutas;
}

module.exports = { crearSolicitudRutas };
