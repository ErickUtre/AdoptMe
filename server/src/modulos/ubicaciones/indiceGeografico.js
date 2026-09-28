const CLAVE_USUARIOS = 'geo:usuarios';
const CLAVE_ADOPCIONES = 'geo:adopciones';

function miembro(prefijo, id) {
  return `${prefijo}:${id}`;
}

function idDesdeMiembro(valor) {
  return Number(String(valor).split(':')[1]);
}

function crearIndiceGeografico(redis) {
  async function agregar(clave, prefijo, id, { latitud, longitud }) {
    await redis.sendCommand(['GEOADD', clave, String(longitud), String(latitud), miembro(prefijo, id)]);
  }

  async function eliminar(clave, prefijo, id) {
    await redis.sendCommand(['ZREM', clave, miembro(prefijo, id)]);
  }

  async function buscar(clave, { latitud, longitud }, { radioMetros, limite }) {
    const respuesta = await redis.sendCommand([
      'GEOSEARCH', clave,
      'FROMLONLAT', String(longitud), String(latitud),
      'BYRADIUS', String(radioMetros), 'm',
      'WITHDIST', 'WITHCOORD',
      'COUNT', String(limite), 'ASC'
    ]);

    return respuesta.map(([valorMiembro, distancia, [lon, lat]]) => ({
      id: idDesdeMiembro(valorMiembro),
      distanciaMetros: Number(distancia),
      latitud: Number(lat),
      longitud: Number(lon)
    }));
  }

  return Object.freeze({
    registrarUsuario: (id, coordenadas) => agregar(CLAVE_USUARIOS, 'usuario', id, coordenadas),
    eliminarUsuario: (id) => eliminar(CLAVE_USUARIOS, 'usuario', id),
    buscarUsuarios: (coordenadas, opciones) => buscar(CLAVE_USUARIOS, coordenadas, opciones),
    registrarAdopcion: (id, coordenadas) => agregar(CLAVE_ADOPCIONES, 'adopcion', id, coordenadas),
    eliminarAdopcion: (id) => eliminar(CLAVE_ADOPCIONES, 'adopcion', id),
    buscarAdopciones: (coordenadas, opciones) => buscar(CLAVE_ADOPCIONES, coordenadas, opciones)
  });
}

module.exports = { crearIndiceGeografico };
