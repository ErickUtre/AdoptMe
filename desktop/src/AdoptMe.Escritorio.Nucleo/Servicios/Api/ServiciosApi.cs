using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Api;

public interface IServicioCuentas
{
    Task<Resultado<Modelos.Sesion>> IniciarSesionAsync(string correo, string contrasena);

    Task<Resultado> RegistrarAsync(SolicitudRegistro solicitud);

    Task<Resultado<ResumenUsuario>> ObtenerResumenAsync(int usuarioId);

    Task<Resultado<Perfil>> ActualizarPerfilAsync(string? nombre, string? telefono);

    Task<Resultado> ActualizarCorreoAsync(string correo);

    Task<Resultado<Ubicacion>> ActualizarUbicacionAsync(Ubicacion ubicacion);

    Task<Resultado<byte[]>> ObtenerFotoPerfilAsync();
}

public interface IServicioAdopciones
{
    Task<Resultado<AdopcionRegistrada>> RegistrarAsync(Mascota mascota, Ubicacion ubicacion);

    Task<Resultado<IReadOnlyList<Adopcion>>> ListarPropiasAsync();

    Task<Resultado<Adopcion>> ObtenerDetalleAsync(int adopcionId);

    Task<Resultado<Adopcion>> ModificarMascotaAsync(int adopcionId, IReadOnlyDictionary<string, string> cambios);

    Task<Resultado> EliminarAsync(int adopcionId);

    Task<Resultado<IReadOnlyList<AdopcionResumen>>> ListarParaReporteAsync(EstadoAdopcion estado);

    Task<Resultado<byte[]>> ObtenerFotoMascotaAsync(int mascotaId);

    Task<Resultado<byte[]>> ObtenerVideoMascotaAsync(int mascotaId);
}

public interface IServicioSolicitudes
{
    Task<Resultado<IReadOnlyList<Solicitud>>> ListarAsync(int adopcionId);

    Task<Resultado> RegistrarAsync(int adopcionId);

    Task<Resultado> AceptarAsync(int adopcionId, int solicitudId);

    Task<Resultado> RechazarAsync(int adopcionId, int solicitudId);
}

public interface IServicioChat
{
    Task<Resultado<IReadOnlyList<Conversacion>>> ListarConversacionesAsync();

    Task<Resultado<IReadOnlyList<Mensaje>>> ListarMensajesAsync(int usuarioId);
}

public interface IServicioNotificaciones
{
    Task<Resultado<IReadOnlyList<Notificacion>>> ListarAsync();

    Task<Resultado> EliminarAsync(int notificacionId);

    Task<Resultado> EliminarTodasAsync();
}

public sealed class ServicioCuentas(ClienteApi api) : IServicioCuentas
{
    public Task<Resultado<Modelos.Sesion>> IniciarSesionAsync(string correo, string contrasena) =>
        api.EnviarAsync<Modelos.Sesion>(HttpMethod.Post, "acceso/iniciar-sesion", new { Correo = correo, Contrasena = contrasena });

    public Task<Resultado> RegistrarAsync(SolicitudRegistro solicitud)
    {
        ArgumentNullException.ThrowIfNull(solicitud);
        return api.EnviarAsync(HttpMethod.Post, "usuarios", new
        {
            solicitud.Nombre,
            solicitud.Telefono,
            solicitud.Ubicacion,
            Acceso = new { solicitud.Correo, solicitud.Contrasena }
        });
    }

    public Task<Resultado<ResumenUsuario>> ObtenerResumenAsync(int usuarioId) =>
        api.ObtenerAsync<ResumenUsuario>($"usuarios/{usuarioId}");

    public Task<Resultado<Perfil>> ActualizarPerfilAsync(string? nombre, string? telefono) =>
        api.EnviarAsync<Perfil>(HttpMethod.Patch, "usuarios/yo", new { Nombre = nombre, Telefono = telefono });

    public Task<Resultado> ActualizarCorreoAsync(string correo) =>
        api.EnviarAsync(HttpMethod.Patch, "acceso", new { Correo = correo });

    public Task<Resultado<Ubicacion>> ActualizarUbicacionAsync(Ubicacion ubicacion) =>
        api.EnviarAsync<Ubicacion>(HttpMethod.Put, "usuarios/yo/ubicacion", ubicacion);

    public Task<Resultado<byte[]>> ObtenerFotoPerfilAsync() => api.ObtenerBytesAsync("usuarios/yo/foto");
}

public sealed class ServicioAdopciones(ClienteApi api) : IServicioAdopciones
{
    public Task<Resultado<AdopcionRegistrada>> RegistrarAsync(Mascota mascota, Ubicacion ubicacion) =>
        api.EnviarAsync<AdopcionRegistrada>(HttpMethod.Post, "adopciones", new { Mascota = mascota, Ubicacion = ubicacion });

    public Task<Resultado<IReadOnlyList<Adopcion>>> ListarPropiasAsync() =>
        api.ObtenerListaAsync<Adopcion>("adopciones/propias");

    public Task<Resultado<Adopcion>> ObtenerDetalleAsync(int adopcionId) =>
        api.ObtenerAsync<Adopcion>($"adopciones/{adopcionId}");

    public Task<Resultado<Adopcion>> ModificarMascotaAsync(int adopcionId, IReadOnlyDictionary<string, string> cambios) =>
        api.EnviarAsync<Adopcion>(HttpMethod.Patch, $"adopciones/{adopcionId}", new { Mascota = cambios });

    public Task<Resultado> EliminarAsync(int adopcionId) =>
        api.EnviarAsync(HttpMethod.Delete, $"adopciones/{adopcionId}");

    public Task<Resultado<IReadOnlyList<AdopcionResumen>>> ListarParaReporteAsync(EstadoAdopcion estado)
    {
        var filtro = estado == EstadoAdopcion.Adoptada ? "adoptada" : "disponible";
        return api.ObtenerListaAsync<AdopcionResumen>($"adopciones?estado={filtro}");
    }

    public Task<Resultado<byte[]>> ObtenerFotoMascotaAsync(int mascotaId) => api.ObtenerBytesAsync($"mascotas/{mascotaId}/foto");

    public Task<Resultado<byte[]>> ObtenerVideoMascotaAsync(int mascotaId) => api.ObtenerBytesAsync($"mascotas/{mascotaId}/video");
}

public sealed class ServicioSolicitudes(ClienteApi api) : IServicioSolicitudes
{
    public Task<Resultado<IReadOnlyList<Solicitud>>> ListarAsync(int adopcionId) =>
        api.ObtenerListaAsync<Solicitud>($"adopciones/{adopcionId}/solicitudes");

    public Task<Resultado> RegistrarAsync(int adopcionId) =>
        api.EnviarAsync(HttpMethod.Post, $"adopciones/{adopcionId}/solicitudes");

    public Task<Resultado> AceptarAsync(int adopcionId, int solicitudId) =>
        api.EnviarAsync(HttpMethod.Post, $"adopciones/{adopcionId}/solicitudes/{solicitudId}/aceptacion");

    public Task<Resultado> RechazarAsync(int adopcionId, int solicitudId) =>
        api.EnviarAsync(HttpMethod.Delete, $"adopciones/{adopcionId}/solicitudes/{solicitudId}");
}

public sealed class ServicioChat(ClienteApi api) : IServicioChat
{
    public Task<Resultado<IReadOnlyList<Conversacion>>> ListarConversacionesAsync() =>
        api.ObtenerListaAsync<Conversacion>("chat/conversaciones");

    public Task<Resultado<IReadOnlyList<Mensaje>>> ListarMensajesAsync(int usuarioId) =>
        api.ObtenerListaAsync<Mensaje>($"chat/conversaciones/{usuarioId}/mensajes");
}

public sealed class ServicioNotificaciones(ClienteApi api) : IServicioNotificaciones
{
    public Task<Resultado<IReadOnlyList<Notificacion>>> ListarAsync() =>
        api.ObtenerListaAsync<Notificacion>("notificaciones");

    public Task<Resultado> EliminarAsync(int notificacionId) =>
        api.EnviarAsync(HttpMethod.Delete, $"notificaciones/{notificacionId}");

    public Task<Resultado> EliminarTodasAsync() => api.EnviarAsync(HttpMethod.Delete, "notificaciones");
}
