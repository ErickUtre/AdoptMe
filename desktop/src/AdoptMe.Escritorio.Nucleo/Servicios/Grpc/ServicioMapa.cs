using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using Grpc.Core;
using AdopcionCercanaGrpc = AdoptMe.Escritorio.Grpc.Ubicacion.AdopcionCercana;
using ClienteUbicacion = AdoptMe.Escritorio.Grpc.Ubicacion.ServicioUbicacion.ServicioUbicacionClient;
using CoordenadasGrpc = AdoptMe.Escritorio.Grpc.Ubicacion.Coordenadas;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Grpc;

public interface IServicioMapa
{
    Task<Resultado<IReadOnlyList<AdopcionCercana>>> ObtenerCercanasAsync(Modelos.Coordenadas centro, CancellationToken cancelacion = default);
}

public sealed class ServicioMapa(CanalGrpc canal) : IServicioMapa
{
    private readonly ClienteUbicacion cliente = new(canal.Canal);

    public async Task<Resultado<IReadOnlyList<AdopcionCercana>>> ObtenerCercanasAsync(Modelos.Coordenadas centro, CancellationToken cancelacion = default)
    {
        ArgumentNullException.ThrowIfNull(centro);
        try
        {
            var respuesta = await cliente.ObtenerAdopcionesCercanasAsync(
                new CoordenadasGrpc { Latitud = centro.Latitud, Longitud = centro.Longitud },
                canal.Encabezados(),
                cancellationToken: cancelacion).ConfigureAwait(false);
            IReadOnlyList<AdopcionCercana> adopciones = respuesta.Resultados.Select(Convertir).ToList();
            return Resultado.Correcto<IReadOnlyList<AdopcionCercana>>(adopciones);
        }
        catch (RpcException excepcion) when (excepcion.StatusCode != StatusCode.Cancelled)
        {
            return Resultado.Fallo<IReadOnlyList<AdopcionCercana>>(CanalGrpc.TraducirError(excepcion));
        }
    }

    private static AdopcionCercana Convertir(AdopcionCercanaGrpc origen) => new(
        origen.AdopcionId,
        origen.PublicadorId,
        origen.DistanciaMetros,
        new Modelos.Coordenadas(origen.Latitud, origen.Longitud),
        new Mascota
        {
            MascotaID = origen.Mascota.MascotaId,
            Nombre = origen.Mascota.Nombre,
            Especie = origen.Mascota.Especie,
            Raza = origen.Mascota.Raza,
            Edad = origen.Mascota.Edad,
            Sexo = origen.Mascota.Sexo,
            Tamano = origen.Mascota.Tamano,
            Descripcion = origen.Mascota.Descripcion
        });
}
