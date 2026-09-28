const { ErrorNoEncontrado, ErrorProhibido, ErrorValidacion } = require('../../compartido/errores');
const { exigirUbicacion, exigirCoordenadas } = require('../../compartido/validacion');
const { exigirMascota, cambiosMascota } = require('./mascota.validacion');
const { TIPOS_NOTIFICACION, REFERENCIAS_NOTIFICACION } = require('../notificaciones/notificacion.servicio');

const RADIO_BUSQUEDA_ADOPCIONES_METROS = 5000;
const LIMITE_ADOPCIONES_CERCANAS = 50;
const RADIO_NOTIFICACION_METROS = 10000;
const LIMITE_USUARIOS_NOTIFICADOS = 100;

const ESTADOS_FILTRO = Object.freeze({ disponible: false, adoptada: true });

function crearAdopcionServicio({
  adopcionRepositorio, indiceGeografico, notificacionServicio, multimediaServicio, transaccion, registro
}) {
  async function exigirAdopcion(adopcionId, transaction) {
    const adopcion = await adopcionRepositorio.buscarPorId(adopcionId, transaction);
    if (!adopcion) {
      throw new ErrorNoEncontrado('Adopción no encontrada');
    }
    return adopcion;
  }

  async function exigirPropia(sesion, adopcionId, transaction) {
    const adopcion = await exigirAdopcion(adopcionId, transaction);
    if (adopcion.PublicadorID !== sesion.usuarioId) {
      throw new ErrorProhibido('La adopción no pertenece al usuario');
    }
    return adopcion;
  }

  async function avisarUsuariosCercanos(adopcion, nombreMascota, coordenadas) {
    const cercanos = await indiceGeografico.buscarUsuarios(coordenadas, {
      radioMetros: RADIO_NOTIFICACION_METROS,
      limite: LIMITE_USUARIOS_NOTIFICADOS
    });

    await notificacionServicio.notificarVarios(cercanos
      .filter(({ id }) => id !== adopcion.PublicadorID)
      .map(({ id, distanciaMetros }) => ({
        usuarioId: id,
        contenido: {
          titulo: 'Nueva adopción cercana',
          mensaje: `¡${nombreMascota} está disponible para adopción a ${(distanciaMetros / 1000).toFixed(2)} km de ti!`,
          tipo: TIPOS_NOTIFICACION.ADOPCION_CERCANA,
          referenciaId: adopcion.AdopcionID,
          referenciaTipo: REFERENCIAS_NOTIFICACION.ADOPCION
        }
      })));
  }

  async function registrar(sesion, datos) {
    const mascota = exigirMascota(datos?.Mascota);
    const ubicacion = exigirUbicacion(datos?.Ubicacion);
    const coordenadas = { latitud: ubicacion.Latitud, longitud: ubicacion.Longitud };

    const adopcion = await transaccion((transaction) => adopcionRepositorio.crear(
      { mascota, ubicacion, publicadorId: sesion.usuarioId },
      transaction
    ));

    try {
      await indiceGeografico.registrarAdopcion(adopcion.AdopcionID, coordenadas);
      await avisarUsuariosCercanos(adopcion, mascota.Nombre, coordenadas);
    } catch (error) {
      registro.error('No se pudo difundir la nueva adopción', { adopcionId: adopcion.AdopcionID, causa: error.message });
    }

    return { AdopcionID: adopcion.AdopcionID, MascotaID: adopcion.MascotaID };
  }

  async function obtenerDetalle(adopcionId) {
    const adopcion = await adopcionRepositorio.buscarDetalle(adopcionId);
    if (!adopcion) {
      throw new ErrorNoEncontrado('Adopción no encontrada');
    }
    return adopcion;
  }

  function listarParaReporte(estado) {
    if (estado !== undefined && !(estado in ESTADOS_FILTRO)) {
      throw new ErrorValidacion(`estado debe ser uno de: ${Object.keys(ESTADOS_FILTRO).join(', ')}`);
    }
    return adopcionRepositorio.listarPorEstado(ESTADOS_FILTRO[estado]);
  }

  async function modificar(sesion, adopcionId, datos) {
    const mascota = cambiosMascota(datos?.Mascota);
    const ubicacion = datos?.Ubicacion ? exigirUbicacion(datos.Ubicacion) : null;
    if (Object.keys(mascota).length === 0 && !ubicacion) {
      throw new ErrorValidacion('No se proporcionaron datos para actualizar');
    }

    const adopcion = await transaccion(async (transaction) => {
      const propia = await exigirPropia(sesion, adopcionId, transaction);
      if (Object.keys(mascota).length > 0) {
        await adopcionRepositorio.actualizarMascota(propia.MascotaID, mascota, transaction);
      }
      if (ubicacion) {
        await adopcionRepositorio.actualizarUbicacion(propia.UbicacionID, ubicacion, transaction);
      }
      return propia;
    });

    if (ubicacion && !adopcion.Estado) {
      await indiceGeografico.registrarAdopcion(adopcionId, { latitud: ubicacion.Latitud, longitud: ubicacion.Longitud });
    }
    return obtenerDetalle(adopcionId);
  }

  async function eliminar(sesion, adopcionId) {
    const adopcion = await exigirPropia(sesion, adopcionId);
    const archivos = await multimediaServicio.listarArchivosDeMascota(adopcion.MascotaID);
    await transaccion((transaction) => adopcionRepositorio.eliminar(adopcion, transaction));
    await indiceGeografico.eliminarAdopcion(adopcionId);
    await multimediaServicio.eliminarArchivos(archivos);
  }

  async function marcarAdoptada(adopcionId, transaction) {
    await adopcionRepositorio.actualizarEstado(adopcionId, true, transaction);
  }

  async function retirarDelMapa(adopcionId) {
    await indiceGeografico.eliminarAdopcion(adopcionId);
  }

  async function buscarCercanas(sesion, coordenadasSolicitadas) {
    const coordenadas = exigirCoordenadas(coordenadasSolicitadas?.latitud, coordenadasSolicitadas?.longitud);
    const puntos = await indiceGeografico.buscarAdopciones(coordenadas, {
      radioMetros: RADIO_BUSQUEDA_ADOPCIONES_METROS,
      limite: LIMITE_ADOPCIONES_CERCANAS
    });
    if (puntos.length === 0) {
      return [];
    }

    const puntosPorId = new Map(puntos.map((punto) => [punto.id, punto]));
    const adopciones = await adopcionRepositorio.listarDisponiblesPorIds([...puntosPorId.keys()], sesion.usuarioId);

    return adopciones
      .map((adopcion) => ({ adopcion, punto: puntosPorId.get(adopcion.AdopcionID) }))
      .sort((a, b) => a.punto.distanciaMetros - b.punto.distanciaMetros)
      .map(({ adopcion, punto }) => ({
        adopcionId: adopcion.AdopcionID,
        publicadorId: adopcion.PublicadorID,
        distanciaMetros: punto.distanciaMetros,
        latitud: punto.latitud,
        longitud: punto.longitud,
        mascota: {
          mascotaId: adopcion.Mascota.MascotaID,
          nombre: adopcion.Mascota.Nombre,
          especie: adopcion.Mascota.Especie,
          raza: adopcion.Mascota.Raza,
          edad: adopcion.Mascota.Edad,
          sexo: adopcion.Mascota.Sexo,
          tamano: adopcion.Mascota.Tamaño,
          descripcion: adopcion.Mascota.Descripcion ?? ''
        }
      }));
  }

  return Object.freeze({
    registrar,
    obtenerDetalle,
    listarPropias: (sesion) => adopcionRepositorio.listarPorPublicador(sesion.usuarioId),
    listarParaReporte,
    modificar,
    eliminar,
    exigirAdopcion,
    exigirPropia,
    marcarAdoptada,
    retirarDelMapa,
    buscarCercanas
  });
}

module.exports = { crearAdopcionServicio };
