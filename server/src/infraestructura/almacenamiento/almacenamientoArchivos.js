const fs = require('node:fs');
const fsp = require('node:fs/promises');
const path = require('node:path');
const crypto = require('node:crypto');

const PREFIJO_HISTORICO = /^\/?multimedia\//;

function crearAlmacenamientoArchivos({ directorioRaiz }) {
  const raiz = path.resolve(directorioRaiz);

  function rutaAbsoluta(rutaRelativa) {
    const destino = path.resolve(raiz, rutaRelativa.replace(PREFIJO_HISTORICO, ''));
    if (destino !== raiz && !destino.startsWith(raiz + path.sep)) {
      throw new Error('Ruta de archivo fuera del almacenamiento');
    }
    return destino;
  }

  async function crearEscritura(carpeta, extension) {
    const nombre = `${Date.now()}-${crypto.randomUUID()}${extension}`;
    const rutaRelativa = path.posix.join(carpeta, nombre);
    const destino = rutaAbsoluta(rutaRelativa);
    await fsp.mkdir(path.dirname(destino), { recursive: true });
    return { rutaRelativa, flujo: fs.createWriteStream(destino) };
  }

  async function eliminar(rutaRelativa) {
    if (!rutaRelativa) {
      return;
    }
    await fsp.rm(rutaAbsoluta(rutaRelativa), { force: true });
  }

  async function describir(rutaRelativa) {
    try {
      const destino = rutaAbsoluta(rutaRelativa);
      const estadisticas = await fsp.stat(destino);
      return estadisticas.isFile() ? { rutaAbsoluta: destino, tamano: estadisticas.size } : null;
    } catch {
      return null;
    }
  }

  function leer(rutaRelativa, rango) {
    return fs.createReadStream(rutaAbsoluta(rutaRelativa), rango);
  }

  return Object.freeze({ crearEscritura, eliminar, describir, leer });
}

module.exports = { crearAlmacenamientoArchivos };
