const { Router } = require('express');

function crearMultimediaRutas({ multimediaControlador, autenticacion }) {
  const rutas = Router();

  rutas.get('/usuarios/yo/foto', autenticacion.requerirSesion, multimediaControlador.fotoPropia);
  rutas.get('/mascotas/:id/foto', autenticacion.requerirSesion, multimediaControlador.fotoMascota);
  rutas.get('/mascotas/:id/video', autenticacion.requerirSesion, multimediaControlador.videoMascota);

  return rutas;
}

module.exports = { crearMultimediaRutas };
