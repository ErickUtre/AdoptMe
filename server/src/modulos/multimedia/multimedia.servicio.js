const path = require('node:path');
const { Readable } = require('node:stream');
const { pipeline } = require('node:stream/promises');
const { ErrorNoEncontrado, ErrorProhibido, ErrorValidacion } = require('../../compartido/errores');
const { exigirIdentificador, exigirTexto } = require('../../compartido/validacion');
const { TIPOS_ARCHIVO, PROPIETARIOS } = require('./tiposArchivo');

function crearMultimediaServicio({ multimediaRepositorio, adopcionRepositorio, almacenamiento, transaccion, registro }) {
  function validarExtension(tipo, nombreArchivo) {
    const extension = path.extname(exigirTexto(nombreArchivo, 'nombreArchivo', 255)).toLowerCase();
    if (!tipo.extensiones.includes(extension)) {
      throw new ErrorValidacion(`Formato no permitido. Solo se permiten: ${tipo.extensiones.join(', ')}`);
    }
    return extension;
  }

  async function resolverReferencia(tipo, sesion, idReferencia) {
    if (tipo.propietario === PROPIETARIOS.USUARIO) {
      return sesion.usuarioId;
    }
    const mascotaId = exigirIdentificador(idReferencia, 'idReferencia');
    const adopcion = await adopcionRepositorio.buscarPorMascota(mascotaId);
    if (!adopcion) {
      throw new ErrorNoEncontrado('Mascota no encontrada');
    }
    if (adopcion.PublicadorID !== sesion.usuarioId) {
      throw new ErrorProhibido('La mascota no pertenece al usuario');
    }
    return mascotaId;
  }

  async function* limitarTamano(fragmentos, tamanoMaximoBytes) {
    let acumulado = 0;
    for await (const fragmento of fragmentos) {
      acumulado += fragmento.length;
      if (acumulado > tamanoMaximoBytes) {
        throw new ErrorValidacion(`El archivo excede el tamaño máximo de ${tamanoMaximoBytes / (1024 * 1024)} MB`);
      }
      yield fragmento;
    }
    if (acumulado === 0) {
      throw new ErrorValidacion('El archivo está vacío');
    }
  }

  async function eliminarArchivos(rutas) {
    await Promise.all(rutas.map(async (ruta) => {
      try {
        await almacenamiento.eliminar(ruta);
      } catch (error) {
        registro.advertencia('No se pudo eliminar un archivo', { ruta, causa: error.message });
      }
    }));
  }

  async function recibirArchivo(tipo, { sesion, metadatos, fragmentos }) {
    const extension = validarExtension(tipo, metadatos?.nombreArchivo);
    const referenciaId = await resolverReferencia(tipo, sesion, metadatos?.idReferencia);

    const { rutaRelativa, flujo } = await almacenamiento.crearEscritura(tipo.carpeta, extension);
    try {
      await pipeline(Readable.from(limitarTamano(fragmentos, tipo.tamanoMaximoBytes)), flujo);
    } catch (error) {
      await eliminarArchivos([rutaRelativa]);
      throw error;
    }

    const anteriores = await transaccion(async (transaction) => {
      const rutas = await multimediaRepositorio.listarRutas(tipo, referenciaId, transaction);
      await multimediaRepositorio.reemplazar(tipo, referenciaId, rutaRelativa, transaction);
      return rutas;
    });
    await eliminarArchivos(anteriores);

    return { exito: true, mensaje: 'Archivo guardado correctamente' };
  }

  async function obtenerArchivo(tipo, referenciaId) {
    const ruta = await multimediaRepositorio.obtenerRuta(tipo, referenciaId);
    const descriptor = ruta ? await almacenamiento.describir(ruta) : null;
    if (!descriptor) {
      throw new ErrorNoEncontrado('Archivo no encontrado');
    }
    return { rutaRelativa: ruta, rutaAbsoluta: descriptor.rutaAbsoluta, tamano: descriptor.tamano };
  }

  async function listarArchivosDeMascota(mascotaId) {
    const fotos = await multimediaRepositorio.listarRutas(TIPOS_ARCHIVO.FOTO_MASCOTA, mascotaId);
    const videos = await multimediaRepositorio.listarRutas(TIPOS_ARCHIVO.VIDEO_MASCOTA, mascotaId);
    return [...fotos, ...videos];
  }

  return Object.freeze({
    recibirArchivo,
    obtenerArchivo,
    abrirLectura: (ruta, rango) => almacenamiento.leer(ruta, rango),
    listarArchivosDeMascota,
    eliminarArchivos
  });
}

module.exports = { crearMultimediaServicio };
