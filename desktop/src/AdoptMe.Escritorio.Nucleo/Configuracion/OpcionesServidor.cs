using System.ComponentModel.DataAnnotations;

namespace AdoptMe.Escritorio.Nucleo.Configuracion;

public sealed class OpcionesServidor
{
    public const string Seccion = "Servidor";

    [Required]
    public Uri UrlApi { get; set; } = new("http://localhost:8080/api/");

    [Required]
    public Uri UrlTiempoReal { get; set; } = new("http://localhost:8080/");

    [Required]
    public Uri UrlGrpc { get; set; } = new("http://localhost:50051");

    public TimeSpan TiempoEsperaPeticiones { get; set; } = TimeSpan.FromSeconds(30);
}

public sealed class OpcionesGeolocalizacion
{
    public const string Seccion = "Geolocalizacion";

    [Required]
    public Uri UrlUbicacionPorIp { get; set; } = new("http://ip-api.com/json");

    [Required]
    public Uri UrlGeocodificacionInversa { get; set; } = new("https://nominatim.openstreetmap.org/reverse");

    public string AgenteUsuario { get; set; } = "AdoptMe-Escritorio/2.0";
}
