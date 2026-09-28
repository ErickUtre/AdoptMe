function crearNotificacionGrpc({ notificacionServicio, autenticacionGrpc }) {
  return {
    EscucharNotificaciones: autenticacionGrpc.protegerFlujoSalida((llamada) => {
      const cancelar = notificacionServicio.suscribir(llamada.sesion.usuarioId, (notificacion) => llamada.write(notificacion));
      const finalizar = () => cancelar();
      llamada.on('cancelled', finalizar);
      llamada.on('close', finalizar);
      llamada.on('error', finalizar);
    })
  };
}

module.exports = { crearNotificacionGrpc };
