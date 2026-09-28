const { TIPOS_ARCHIVO } = require('./tiposArchivo');

function crearMultimediaRepositorio({ FotoUsuario, FotoMascota, VideoMascota }) {
  const destinos = {
    [TIPOS_ARCHIVO.FOTO_USUARIO.clave]: { modelo: FotoUsuario, llave: 'UsuarioID', campoRuta: 'UrlFoto' },
    [TIPOS_ARCHIVO.FOTO_MASCOTA.clave]: { modelo: FotoMascota, llave: 'MascotaID', campoRuta: 'UrlFoto' },
    [TIPOS_ARCHIVO.VIDEO_MASCOTA.clave]: { modelo: VideoMascota, llave: 'MascotaID', campoRuta: 'UrlVideo' }
  };

  async function listarRutas(tipo, referenciaId, transaction) {
    const { modelo, llave, campoRuta } = destinos[tipo.clave];
    const registros = await modelo.findAll({ where: { [llave]: referenciaId }, transaction });
    return registros.map((registro) => registro[campoRuta]);
  }

  async function reemplazar(tipo, referenciaId, rutaRelativa, transaction) {
    const { modelo, llave, campoRuta } = destinos[tipo.clave];
    await modelo.destroy({ where: { [llave]: referenciaId }, transaction });
    await modelo.create({ [llave]: referenciaId, [campoRuta]: rutaRelativa }, { transaction });
  }

  async function obtenerRuta(tipo, referenciaId) {
    const [ruta] = await listarRutas(tipo, referenciaId);
    return ruta ?? null;
  }

  return Object.freeze({ listarRutas, reemplazar, obtenerRuta });
}

module.exports = { crearMultimediaRepositorio };
