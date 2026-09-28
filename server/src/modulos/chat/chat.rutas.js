const { Router } = require('express');

function crearChatRutas({ chatControlador, autenticacion }) {
  const rutas = Router();

  rutas.use(autenticacion.requerirSesion);
  rutas.get('/conversaciones', chatControlador.listarConversaciones);
  rutas.get('/conversaciones/:usuarioId/mensajes', chatControlador.listarMensajes);
  rutas.post('/conversaciones/:usuarioId/mensajes', chatControlador.enviar);

  return rutas;
}

module.exports = { crearChatRutas };
