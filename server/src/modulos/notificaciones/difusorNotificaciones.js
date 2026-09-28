function crearDifusorNotificaciones() {
  const suscriptores = new Map();

  function suscribir(usuarioId, alRecibir) {
    const conjunto = suscriptores.get(usuarioId) ?? new Set();
    conjunto.add(alRecibir);
    suscriptores.set(usuarioId, conjunto);

    return function cancelar() {
      conjunto.delete(alRecibir);
      if (conjunto.size === 0) {
        suscriptores.delete(usuarioId);
      }
    };
  }

  function publicar(usuarioId, notificacion) {
    for (const alRecibir of suscriptores.get(usuarioId) ?? []) {
      alRecibir(notificacion);
    }
  }

  return Object.freeze({ suscribir, publicar });
}

module.exports = { crearDifusorNotificaciones };
