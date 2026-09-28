const { crearNotificacionServicio } = require('../../../src/modulos/notificaciones/notificacion.servicio');
const { crearDifusorNotificaciones } = require('../../../src/modulos/notificaciones/difusorNotificaciones');
const { ErrorNoEncontrado } = require('../../../src/compartido/errores');

function registroPersistido(datos, id) {
  return { NotificacionID: id, ...datos };
}

describe('notificacionServicio', () => {
  test('persiste y difunde solo a los suscriptores del destinatario', async () => {
    const notificacionRepositorio = { crear: jest.fn().mockImplementation(async (datos) => registroPersistido(datos, 1)) };
    const servicio = crearNotificacionServicio({ notificacionRepositorio, difusor: crearDifusorNotificaciones() });
    const recibidasPorDestinatario = [];
    const recibidasPorOtro = [];
    servicio.suscribir(5, (notificacion) => recibidasPorDestinatario.push(notificacion));
    servicio.suscribir(6, (notificacion) => recibidasPorOtro.push(notificacion));

    await servicio.notificar(5, { titulo: 'T', mensaje: 'M', tipo: 'AdopcionCercana', referenciaId: 9, referenciaTipo: 'Adopcion' });

    expect(recibidasPorOtro).toHaveLength(0);
    expect(recibidasPorDestinatario).toEqual([expect.objectContaining({
      notificacionId: 1, titulo: 'T', referenciaId: 9, referenciaTipo: 'Adopcion'
    })]);
  });

  test('deja de difundir cuando se cancela la suscripción', async () => {
    const difusor = crearDifusorNotificaciones();
    const recibidas = [];
    const cancelar = difusor.suscribir(1, (n) => recibidas.push(n));
    cancelar();
    difusor.publicar(1, { titulo: 'x' });
    expect(recibidas).toHaveLength(0);
  });

  test('informa cuando la notificación a eliminar no pertenece al usuario', async () => {
    const notificacionRepositorio = { eliminar: jest.fn().mockResolvedValue(0) };
    const servicio = crearNotificacionServicio({ notificacionRepositorio, difusor: crearDifusorNotificaciones() });
    await expect(servicio.eliminar(1, 50)).rejects.toBeInstanceOf(ErrorNoEncontrado);
  });
});
