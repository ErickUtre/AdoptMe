const { ErrorValidacion } = require('../../compartido/errores');
const { TIPOS_ARCHIVO } = require('./tiposArchivo');

async function separarFlujo(llamada) {
  const iterador = llamada[Symbol.asyncIterator]();
  const primero = await iterador.next();
  if (primero.done || primero.value.contenido !== 'metadatos') {
    throw new ErrorValidacion('El primer mensaje debe contener los metadatos del archivo');
  }

  async function* fragmentos() {
    for (let actual = await iterador.next(); !actual.done; actual = await iterador.next()) {
      if (actual.value.contenido !== 'datos') {
        throw new ErrorValidacion('Los metadatos solo pueden enviarse una vez');
      }
      yield actual.value.datos;
    }
  }

  return { metadatos: primero.value.metadatos, fragmentos: fragmentos() };
}

function crearMultimediaGrpc({ multimediaServicio, autenticacionGrpc }) {
  const manejadorPara = (tipo) => autenticacionGrpc.protegerUnaria(async (llamada) => {
    const { metadatos, fragmentos } = await separarFlujo(llamada);
    return multimediaServicio.recibirArchivo(tipo, { sesion: llamada.sesion, metadatos, fragmentos });
  });

  return {
    SubirFotoUsuario: manejadorPara(TIPOS_ARCHIVO.FOTO_USUARIO),
    SubirFotoMascota: manejadorPara(TIPOS_ARCHIVO.FOTO_MASCOTA),
    SubirVideoMascota: manejadorPara(TIPOS_ARCHIVO.VIDEO_MASCOTA)
  };
}

module.exports = { crearMultimediaGrpc };
