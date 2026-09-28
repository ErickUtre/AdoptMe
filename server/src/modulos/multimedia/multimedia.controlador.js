const { pipeline } = require('node:stream/promises');
const { exigirIdentificador } = require('../../compartido/validacion');
const { TIPOS_ARCHIVO } = require('./tiposArchivo');

const PATRON_RANGO = /^bytes=(\d*)-(\d*)$/;

function interpretarRango(encabezado, tamano) {
  const coincidencia = PATRON_RANGO.exec(encabezado ?? '');
  if (!coincidencia) {
    return null;
  }
  const [, inicioTexto, finTexto] = coincidencia;
  const inicio = inicioTexto === '' ? Math.max(tamano - Number(finTexto), 0) : Number(inicioTexto);
  const fin = inicioTexto === '' || finTexto === '' ? tamano - 1 : Math.min(Number(finTexto), tamano - 1);
  return inicio <= fin && inicio < tamano ? { start: inicio, end: fin } : { invalido: true };
}

function crearMultimediaControlador({ multimediaServicio }) {
  async function enviarImagen(res, tipo, referenciaId) {
    const archivo = await multimediaServicio.obtenerArchivo(tipo, referenciaId);
    res.sendFile(archivo.rutaAbsoluta);
  }

  async function transmitirVideo(req, res) {
    const archivo = await multimediaServicio.obtenerArchivo(TIPOS_ARCHIVO.VIDEO_MASCOTA, exigirIdentificador(req.params.id, 'id'));
    const rango = interpretarRango(req.headers.range, archivo.tamano);

    res.set({ 'Accept-Ranges': 'bytes', 'Content-Type': 'video/mp4' });

    if (rango?.invalido) {
      res.status(416).set('Content-Range', `bytes */${archivo.tamano}`).end();
      return;
    }

    if (rango) {
      res.status(206).set({
        'Content-Range': `bytes ${rango.start}-${rango.end}/${archivo.tamano}`,
        'Content-Length': rango.end - rango.start + 1
      });
    } else {
      res.status(200).set('Content-Length', archivo.tamano);
    }
    await pipeline(multimediaServicio.abrirLectura(archivo.rutaRelativa, rango ?? undefined), res);
  }

  return Object.freeze({
    fotoPropia: (req, res) => enviarImagen(res, TIPOS_ARCHIVO.FOTO_USUARIO, req.sesion.usuarioId),
    fotoMascota: (req, res) => enviarImagen(res, TIPOS_ARCHIVO.FOTO_MASCOTA, exigirIdentificador(req.params.id, 'id')),
    videoMascota: transmitirVideo
  });
}

module.exports = { crearMultimediaControlador };
