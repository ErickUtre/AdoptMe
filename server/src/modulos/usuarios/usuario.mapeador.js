function mapearUbicacion(ubicacion) {
  if (!ubicacion) {
    return null;
  }
  return {
    UbicacionID: ubicacion.UbicacionID,
    Latitud: ubicacion.Latitud,
    Longitud: ubicacion.Longitud,
    Ciudad: ubicacion.Ciudad,
    Estado: ubicacion.Estado,
    Pais: ubicacion.Pais
  };
}

function mapearPerfil(usuario, acceso = usuario.Acceso) {
  return {
    UsuarioID: usuario.UsuarioID,
    Nombre: usuario.Nombre,
    Telefono: usuario.Telefono,
    FechaRegistro: usuario.FechaRegistro,
    Ubicacion: mapearUbicacion(usuario.Ubicacion),
    Acceso: {
      Correo: acceso.Correo,
      EsAdmin: acceso.EsAdmin
    }
  };
}

module.exports = { mapearPerfil, mapearUbicacion };
