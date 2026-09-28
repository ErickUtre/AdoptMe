using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Modelos;
using Microsoft.Extensions.Options;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;

public interface IServicioGeolocalizacion
{
    Task<Resultado<Ubicacion>> ObtenerUbicacionAproximadaAsync();

    Task<Resultado<Ubicacion>> ObtenerDireccionAsync(Coordenadas coordenadas);
}

public sealed class ServicioGeolocalizacion(HttpClient http, IOptions<OpcionesGeolocalizacion> opciones) : IServicioGeolocalizacion
{
    public Task<Resultado<Ubicacion>> ObtenerUbicacionAproximadaAsync() =>
        ConsultarAsync<RespuestaIp>(opciones.Value.UrlUbicacionPorIp, respuesta => new Ubicacion
        {
            Latitud = respuesta.Lat,
            Longitud = respuesta.Lon,
            Ciudad = respuesta.City,
            Estado = respuesta.RegionName,
            Pais = respuesta.Country
        });

    public Task<Resultado<Ubicacion>> ObtenerDireccionAsync(Coordenadas coordenadas)
    {
        ArgumentNullException.ThrowIfNull(coordenadas);
        var consulta = string.Create(CultureInfo.InvariantCulture, $"?lat={coordenadas.Latitud}&lon={coordenadas.Longitud}&format=json");
        return ConsultarAsync<RespuestaGeocodificacion>(new Uri(opciones.Value.UrlGeocodificacionInversa + consulta), respuesta => new Ubicacion
        {
            Latitud = coordenadas.Latitud,
            Longitud = coordenadas.Longitud,
            Ciudad = respuesta.Address?.City ?? respuesta.Address?.Town ?? respuesta.Address?.Village,
            Estado = respuesta.Address?.State,
            Pais = respuesta.Address?.Country
        });
    }

    private async Task<Resultado<Ubicacion>> ConsultarAsync<T>(Uri direccion, Func<T, Ubicacion> convertir)
    {
        try
        {
            using var peticion = new HttpRequestMessage(HttpMethod.Get, direccion);
            peticion.Headers.UserAgent.ParseAdd(opciones.Value.AgenteUsuario);
            using var respuesta = await http.SendAsync(peticion).ConfigureAwait(false);
            respuesta.EnsureSuccessStatusCode();
            var contenido = await respuesta.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
            return contenido is null
                ? Resultado.Fallo<Ubicacion>(new ErrorOperacion(TipoError.Desconocido, Textos.ErrorUbicacion))
                : Resultado.Correcto<Ubicacion>(convertir(contenido));
        }
        catch (Exception excepcion) when (excepcion is HttpRequestException or TaskCanceledException or JsonException)
        {
            return Resultado.Fallo<Ubicacion>(new ErrorOperacion(TipoError.Conexion, Textos.ErrorUbicacion));
        }
    }

    private sealed record RespuestaIp(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lon")] double Lon,
        [property: JsonPropertyName("city")] string? City,
        [property: JsonPropertyName("regionName")] string? RegionName,
        [property: JsonPropertyName("country")] string? Country);

    private sealed record RespuestaGeocodificacion([property: JsonPropertyName("address")] Direccion? Address);

    private sealed record Direccion(
        [property: JsonPropertyName("city")] string? City,
        [property: JsonPropertyName("town")] string? Town,
        [property: JsonPropertyName("village")] string? Village,
        [property: JsonPropertyName("state")] string? State,
        [property: JsonPropertyName("country")] string? Country);
}
