using System.Runtime.InteropServices;
using AdoptMe.Escritorio.Nucleo;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;
using Windows.Devices.Geolocation;

namespace AdoptMe.Escritorio.Infraestructura;

public sealed class ProveedorPosicionWindows : IProveedorPosicion
{
    private static readonly TimeSpan AntiguedadMaxima = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan TiempoEspera = TimeSpan.FromSeconds(10);

    public async Task<Resultado<Coordenadas>> ObtenerPosicionAsync(CancellationToken cancelacion = default)
    {
        try
        {
            var acceso = await Geolocator.RequestAccessAsync().AsTask(cancelacion).ConfigureAwait(false);
            if (acceso != GeolocationAccessStatus.Allowed)
            {
                return Resultado.Fallo<Coordenadas>(new ErrorOperacion(TipoError.Prohibido, acceso.ToString()));
            }
            var localizador = new Geolocator { DesiredAccuracy = PositionAccuracy.High };
            var posicion = await localizador.GetGeopositionAsync(AntiguedadMaxima, TiempoEspera).AsTask(cancelacion).ConfigureAwait(false);
            var punto = posicion.Coordinate.Point.Position;
            return Resultado.Correcto(new Coordenadas(punto.Latitude, punto.Longitude));
        }
        catch (Exception excepcion) when (excepcion is COMException or UnauthorizedAccessException or TimeoutException or OperationCanceledException)
        {
            return Resultado.Fallo<Coordenadas>(new ErrorOperacion(TipoError.ServicioNoDisponible, $"{Textos.UbicacionDispositivoNoDisponible} {excepcion.Message}"));
        }
    }
}
