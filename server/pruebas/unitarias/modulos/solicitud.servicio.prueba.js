const { crearSolicitudServicio } = require('../../../src/modulos/solicitudes/solicitud.servicio');
const { ErrorConflicto, ErrorValidacion, ErrorProhibido, ErrorNoEncontrado } = require('../../../src/compartido/errores');
const { registroSilencioso, transaccionDirecta } = require('../apoyo');

function crearDependencias({ adopcion = { AdopcionID: 5, PublicadorID: 1, Estado: false }, existe = false, solicitud = { SolicitudID: 7, AdopcionID: 5, AdoptanteID: 2 } } = {}) {
  const solicitudRepositorio = {
    existe: jest.fn().mockResolvedValue(existe),
    crear: jest.fn().mockResolvedValue({ SolicitudID: 7 }),
    buscarPorId: jest.fn().mockResolvedValue(solicitud),
    listarPorAdopcion: jest.fn().mockResolvedValue([{ SolicitudID: 7, AdopcionID: 5, AdoptanteID: 2, Adoptante: { Nombre: 'Luis' } }]),
    eliminar: jest.fn().mockResolvedValue(),
    eliminarPorAdopcion: jest.fn().mockResolvedValue()
  };
  const adopcionServicio = {
    exigirAdopcion: jest.fn().mockResolvedValue(adopcion),
    exigirPropia: jest.fn().mockResolvedValue(adopcion),
    marcarAdoptada: jest.fn().mockResolvedValue(),
    retirarDelMapa: jest.fn().mockResolvedValue()
  };
  const notificacionServicio = { notificar: jest.fn().mockResolvedValue() };
  const servicio = crearSolicitudServicio({
    solicitudRepositorio, adopcionServicio, notificacionServicio, transaccion: transaccionDirecta, registro: registroSilencioso
  });
  return { servicio, solicitudRepositorio, adopcionServicio, notificacionServicio };
}

describe('solicitudServicio.registrar', () => {
  test('registra la solicitud y avisa al publicador', async () => {
    const { servicio, solicitudRepositorio, notificacionServicio } = crearDependencias();
    await expect(servicio.registrar({ usuarioId: 2 }, 5)).resolves.toEqual({ SolicitudID: 7, AdopcionID: 5, AdoptanteID: 2 });
    expect(solicitudRepositorio.crear).toHaveBeenCalledWith({ AdoptanteID: 2, AdopcionID: 5 });
    expect(notificacionServicio.notificar).toHaveBeenCalledWith(1, expect.objectContaining({ tipo: 'SolicitudRecibida' }));
  });

  test('impide solicitar una adopción propia, ya adoptada o repetida', async () => {
    await expect(crearDependencias().servicio.registrar({ usuarioId: 1 }, 5)).rejects.toBeInstanceOf(ErrorValidacion);
    await expect(crearDependencias({ adopcion: { PublicadorID: 1, Estado: true } }).servicio.registrar({ usuarioId: 2 }, 5))
      .rejects.toBeInstanceOf(ErrorConflicto);
    await expect(crearDependencias({ existe: true }).servicio.registrar({ usuarioId: 2 }, 5)).rejects.toBeInstanceOf(ErrorConflicto);
  });
});

describe('solicitudServicio.aceptar y rechazar', () => {
  test('aceptar marca la adopción, limpia sus solicitudes, la retira del mapa y avisa al adoptante', async () => {
    const { servicio, solicitudRepositorio, adopcionServicio, notificacionServicio } = crearDependencias();
    await servicio.aceptar({ usuarioId: 1 }, 5, 7);
    expect(adopcionServicio.marcarAdoptada).toHaveBeenCalledWith(5, 'transaccion');
    expect(solicitudRepositorio.eliminarPorAdopcion).toHaveBeenCalledWith(5, 'transaccion');
    expect(adopcionServicio.retirarDelMapa).toHaveBeenCalledWith(5);
    expect(notificacionServicio.notificar).toHaveBeenCalledWith(2, expect.objectContaining({ tipo: 'SolicitudAceptada' }));
  });

  test('una solicitud de otra adopción no se puede aceptar', async () => {
    const { servicio } = crearDependencias({ solicitud: { SolicitudID: 7, AdopcionID: 99, AdoptanteID: 2 } });
    await expect(servicio.aceptar({ usuarioId: 1 }, 5, 7)).rejects.toBeInstanceOf(ErrorNoEncontrado);
  });

  test('solo el publicador o el adoptante pueden rechazar', async () => {
    const { servicio, solicitudRepositorio } = crearDependencias();
    await expect(servicio.rechazar({ usuarioId: 3 }, 5, 7)).rejects.toBeInstanceOf(ErrorProhibido);
    await servicio.rechazar({ usuarioId: 2 }, 5, 7);
    expect(solicitudRepositorio.eliminar).toHaveBeenCalledWith(7);
  });

  test('lista las solicitudes con el nombre del adoptante', async () => {
    const { servicio } = crearDependencias();
    await expect(servicio.listar({ usuarioId: 1 }, 5)).resolves.toEqual([
      { SolicitudID: 7, AdopcionID: 5, AdoptanteID: 2, NombreAdoptante: 'Luis' }
    ]);
  });
});
