const { Router } = require('express');

function crearUsuarioRutas({ usuarioControlador, autenticacion }) {
  const rutas = Router();

  rutas.post('/', usuarioControlador.registrar);
  rutas.get('/yo', autenticacion.requerirSesion, usuarioControlador.obtenerPerfilPropio);
  rutas.patch('/yo', autenticacion.requerirSesion, usuarioControlador.actualizarPerfilPropio);
  rutas.put('/yo/ubicacion', autenticacion.requerirSesion, usuarioControlador.actualizarUbicacionPropia);
  rutas.get('/:id', autenticacion.requerirSesion, usuarioControlador.obtenerResumen);

  return rutas;
}

module.exports = { crearUsuarioRutas };
