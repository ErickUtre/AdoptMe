async function sincronizarIndiceGeografico({ usuarioRepositorio, adopcionRepositorio, indiceGeografico, registro }) {
  const [usuarios, adopciones] = await Promise.all([
    usuarioRepositorio.listarConUbicacion(),
    adopcionRepositorio.listarDisponiblesConUbicacion()
  ]);

  const coordenadas = ({ Ubicacion }) => ({ latitud: Ubicacion.Latitud, longitud: Ubicacion.Longitud });

  await Promise.all([
    ...usuarios.map((usuario) => indiceGeografico.registrarUsuario(usuario.UsuarioID, coordenadas(usuario))),
    ...adopciones.map((adopcion) => indiceGeografico.registrarAdopcion(adopcion.AdopcionID, coordenadas(adopcion)))
  ]);

  registro.info('Índice geográfico sincronizado', { usuarios: usuarios.length, adopciones: adopciones.length });
}

module.exports = { sincronizarIndiceGeografico };
