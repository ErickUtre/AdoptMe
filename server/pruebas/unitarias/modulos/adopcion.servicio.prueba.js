const { crearAdopcionServicio } = require('../../../src/modulos/adopciones/adopcion.servicio');
const { ErrorProhibido, ErrorValidacion, ErrorNoEncontrado } = require('../../../src/compartido/errores');
const { registroSilencioso, transaccionDirecta, crearIndiceGeograficoFalso } = require('../apoyo');

const SESION = Object.freeze({ usuarioId: 1, rol: 'Usuario' });

const mascotaValida = Object.freeze({
  Nombre: 'Max', Especie: 'Perro', Raza: 'Mestizo', Edad: '2 años', Sexo: 'Macho', Tamaño: 'Mediano'
});

function crearDependencias({ adopcion = { AdopcionID: 5, MascotaID: 8, UbicacionID: 4, PublicadorID: 1, Estado: false }, cercanos = [], puntos = [], disponibles = [] } = {}) {
  const adopcionRepositorio = {
    crear: jest.fn().mockResolvedValue({ AdopcionID: 5, MascotaID: 8, PublicadorID: 1 }),
    buscarPorId: jest.fn().mockResolvedValue(adopcion),
    buscarDetalle: jest.fn().mockResolvedValue({ AdopcionID: 5 }),
    actualizarMascota: jest.fn().mockResolvedValue(),
    actualizarUbicacion: jest.fn().mockResolvedValue(),
    eliminar: jest.fn().mockResolvedValue(),
    listarDisponiblesPorIds: jest.fn().mockResolvedValue(disponibles),
    listarPorEstado: jest.fn().mockResolvedValue([])
  };
  const indiceGeografico = crearIndiceGeograficoFalso({ usuarios: cercanos, adopciones: puntos });
  const notificacionServicio = { notificarVarios: jest.fn().mockResolvedValue() };
  const multimediaServicio = {
    listarArchivosDeMascota: jest.fn().mockResolvedValue(['fotos/mascotas/a.png']),
    eliminarArchivos: jest.fn().mockResolvedValue()
  };
  const servicio = crearAdopcionServicio({
    adopcionRepositorio, indiceGeografico, notificacionServicio, multimediaServicio, transaccion: transaccionDirecta, registro: registroSilencioso
  });
  return { servicio, adopcionRepositorio, indiceGeografico, notificacionServicio, multimediaServicio };
}

describe('adopcionServicio.registrar', () => {
  test('publica la adopción, la indexa y avisa a los usuarios cercanos excepto al publicador', async () => {
    const { servicio, adopcionRepositorio, indiceGeografico, notificacionServicio } = crearDependencias({
      cercanos: [{ id: 1, distanciaMetros: 0 }, { id: 2, distanciaMetros: 1500 }]
    });

    const resultado = await servicio.registrar(SESION, { Mascota: mascotaValida, Ubicacion: { Latitud: 19.5, Longitud: -96.9 } });

    expect(resultado).toEqual({ AdopcionID: 5, MascotaID: 8 });
    expect(adopcionRepositorio.crear.mock.calls[0][0].publicadorId).toBe(1);
    expect(indiceGeografico.registrarAdopcion).toHaveBeenCalledWith(5, { latitud: 19.5, longitud: -96.9 });
    const [destinatarios] = notificacionServicio.notificarVarios.mock.calls[0];
    expect(destinatarios).toHaveLength(1);
    expect(destinatarios[0]).toMatchObject({ usuarioId: 2, contenido: { referenciaId: 5, tipo: 'AdopcionCercana' } });
    expect(destinatarios[0].contenido.mensaje).toContain('1.50 km');
  });

  test('exige un sexo válido para la mascota', async () => {
    const { servicio } = crearDependencias();
    await expect(servicio.registrar(SESION, { Mascota: { ...mascotaValida, Sexo: 'Otro' }, Ubicacion: { Latitud: 1, Longitud: 1 } }))
      .rejects.toBeInstanceOf(ErrorValidacion);
  });
});

describe('adopcionServicio.modificar y eliminar', () => {
  test('impide modificar adopciones de otros usuarios', async () => {
    const { servicio, adopcionRepositorio } = crearDependencias({ adopcion: { AdopcionID: 5, PublicadorID: 99 } });
    await expect(servicio.modificar(SESION, 5, { Mascota: { Nombre: 'Luna' } })).rejects.toBeInstanceOf(ErrorProhibido);
    expect(adopcionRepositorio.actualizarMascota).not.toHaveBeenCalled();
  });

  test('elimina la adopción, la retira del mapa y borra sus archivos', async () => {
    const { servicio, adopcionRepositorio, indiceGeografico, multimediaServicio } = crearDependencias();
    await servicio.eliminar(SESION, 5);
    expect(adopcionRepositorio.eliminar).toHaveBeenCalled();
    expect(indiceGeografico.eliminarAdopcion).toHaveBeenCalledWith(5);
    expect(multimediaServicio.eliminarArchivos).toHaveBeenCalledWith(['fotos/mascotas/a.png']);
  });

  test('informa cuando la adopción no existe', async () => {
    const { servicio } = crearDependencias({ adopcion: null });
    await expect(servicio.eliminar(SESION, 5)).rejects.toBeInstanceOf(ErrorNoEncontrado);
  });
});

describe('adopcionServicio.buscarCercanas', () => {
  test('combina el índice geográfico con los datos de la mascota ordenando por distancia', async () => {
    const mascota = (id, nombre) => ({ MascotaID: id, Nombre: nombre, Especie: 'Gato', Raza: 'Siamés', Edad: '1', Sexo: 'Hembra', Tamaño: 'Chico', Descripcion: null });
    const { servicio, adopcionRepositorio } = crearDependencias({
      puntos: [
        { id: 10, distanciaMetros: 900, latitud: 19.1, longitud: -96.1 },
        { id: 11, distanciaMetros: 100, latitud: 19.2, longitud: -96.2 }
      ],
      disponibles: [
        { AdopcionID: 10, PublicadorID: 3, Mascota: mascota(1, 'Mia') },
        { AdopcionID: 11, PublicadorID: 4, Mascota: mascota(2, 'Nala') }
      ]
    });

    const resultados = await servicio.buscarCercanas(SESION, { latitud: 19, longitud: -96 });

    expect(adopcionRepositorio.listarDisponiblesPorIds).toHaveBeenCalledWith([10, 11], 1);
    expect(resultados.map((r) => r.adopcionId)).toEqual([11, 10]);
    expect(resultados[0]).toMatchObject({ publicadorId: 4, latitud: 19.2, mascota: { nombre: 'Nala', tamano: 'Chico', descripcion: '' } });
  });

  test('valida el filtro de estado de los reportes', () => {
    const { servicio } = crearDependencias();
    expect(() => servicio.listarParaReporte('inexistente')).toThrow(ErrorValidacion);
  });
});
