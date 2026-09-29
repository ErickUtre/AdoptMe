using System.Text.Json.Serialization;

namespace AdoptMe.Escritorio.Nucleo.Modelos;

public sealed record Coordenadas(double Latitud, double Longitud);

public sealed record UbicacionDetectada(Ubicacion Ubicacion, bool EsPrecisa);

public sealed record Ubicacion
{
    public int? UbicacionID { get; init; }
    public double Latitud { get; init; }
    public double Longitud { get; init; }
    public string? Ciudad { get; init; }
    public string? Estado { get; init; }
    public string? Pais { get; init; }

    [JsonIgnore]
    public Coordenadas Coordenadas => new(Latitud, Longitud);
}

public sealed record DatosAcceso(string Correo, bool EsAdmin);

public sealed record Perfil
{
    public int UsuarioID { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Telefono { get; init; }
    public DateTime? FechaRegistro { get; init; }
    public Ubicacion? Ubicacion { get; init; }
    public DatosAcceso Acceso { get; init; } = new(string.Empty, false);
}

public sealed record Sesion(string Token, bool EsAdmin, Perfil Usuario);

public sealed record ResumenUsuario(int UsuarioID, string Nombre);

public sealed record SolicitudRegistro(string Nombre, string Telefono, string Correo, string Contrasena, Ubicacion? Ubicacion);

public static class SexosMascota
{
    public const string Macho = "Macho";
    public const string Hembra = "Hembra";

    public static IReadOnlyList<string> Todos { get; } = [Macho, Hembra];
}

public sealed record Mascota
{
    public int MascotaID { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Especie { get; init; } = string.Empty;
    public string Raza { get; init; } = string.Empty;
    public string Edad { get; init; } = string.Empty;
    public string Sexo { get; init; } = string.Empty;

    [JsonPropertyName("Tamaño")]
    public string Tamano { get; init; } = string.Empty;

    public string? Descripcion { get; init; }
}

public sealed record Adopcion
{
    public int AdopcionID { get; init; }
    public DateTime? FechaSolicitud { get; init; }
    public bool Estado { get; init; }
    public int MascotaID { get; init; }
    public int PublicadorID { get; init; }
    public Mascota? Mascota { get; init; }
    public Ubicacion? Ubicacion { get; init; }
}

public sealed record AdopcionRegistrada(int AdopcionID, int MascotaID);

public sealed record AdopcionResumen(int AdopcionID, bool Estado, DateTime? FechaSolicitud);

public enum EstadoAdopcion
{
    Disponible,
    Adoptada
}

public sealed record AdopcionCercana(int AdopcionId, int PublicadorId, double DistanciaMetros, Coordenadas Coordenadas, Mascota Mascota);

public sealed record Solicitud(int SolicitudID, int AdopcionID, int AdoptanteID, string NombreAdoptante);

public sealed record Conversacion(int UsuarioID, string Nombre, string UltimoMensaje, DateTime Fecha);

public sealed record Mensaje(int ChatID, int RemitenteID, int DestinatarioID, string Contenido, DateTime FechaEnvio);

public static class TiposNotificacion
{
    public const string AdopcionCercana = "AdopcionCercana";
    public const string SolicitudRecibida = "SolicitudRecibida";
    public const string SolicitudAceptada = "SolicitudAceptada";
}

public sealed record Notificacion
{
    public int NotificacionID { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public DateTime FechaCreacion { get; init; }
    public int? ReferenciaID { get; init; }
    public string? ReferenciaTipo { get; init; }
}
