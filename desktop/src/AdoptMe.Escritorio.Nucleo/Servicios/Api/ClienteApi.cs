using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Api;

public sealed class ClienteApi(HttpClient http)
{
    internal static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public Task<Resultado<T>> ObtenerAsync<T>(string ruta, CancellationToken cancelacion = default) =>
        EjecutarAsync(() => http.GetAsync(ruta, cancelacion), LeerJsonAsync<T>, cancelacion);

    public async Task<Resultado<IReadOnlyList<T>>> ObtenerListaAsync<T>(string ruta, CancellationToken cancelacion = default) =>
        (await ObtenerAsync<List<T>>(ruta, cancelacion).ConfigureAwait(false)).ComoSoloLectura();

    public Task<Resultado<byte[]>> ObtenerBytesAsync(string ruta, CancellationToken cancelacion = default) =>
        EjecutarAsync(() => http.GetAsync(ruta, cancelacion), (contenido, ct) => contenido.ReadAsByteArrayAsync(ct), cancelacion);

    public Task<Resultado<T>> EnviarAsync<T>(HttpMethod metodo, string ruta, object? cuerpo = null, CancellationToken cancelacion = default) =>
        EjecutarAsync(() => http.SendAsync(CrearPeticion(metodo, ruta, cuerpo), cancelacion), LeerJsonAsync<T>, cancelacion);

    public async Task<Resultado> EnviarAsync(HttpMethod metodo, string ruta, object? cuerpo = null, CancellationToken cancelacion = default)
    {
        var resultado = await EjecutarAsync(
            () => http.SendAsync(CrearPeticion(metodo, ruta, cuerpo), cancelacion),
            (_, _) => Task.FromResult(true),
            cancelacion).ConfigureAwait(false);
        return resultado.Exito ? Resultado.Correcto() : Resultado.Fallo(resultado.Error);
    }

    private static HttpRequestMessage CrearPeticion(HttpMethod metodo, string ruta, object? cuerpo) => new(metodo, ruta)
    {
        Content = cuerpo is null ? null : JsonContent.Create(cuerpo, cuerpo.GetType(), options: OpcionesJson)
    };

    private static async Task<T> LeerJsonAsync<T>(HttpContent contenido, CancellationToken cancelacion) =>
        await contenido.ReadFromJsonAsync<T>(OpcionesJson, cancelacion).ConfigureAwait(false)
        ?? throw new JsonException("La respuesta del servidor está vacía.");

    private static async Task<Resultado<T>> EjecutarAsync<T>(
        Func<Task<HttpResponseMessage>> enviar,
        Func<HttpContent, CancellationToken, Task<T>> leer,
        CancellationToken cancelacion)
    {
        try
        {
            using var respuesta = await enviar().ConfigureAwait(false);
            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado.Fallo<T>(await TraducirErrorAsync(respuesta, cancelacion).ConfigureAwait(false));
            }
            return Resultado.Correcto<T>(await leer(respuesta.Content, cancelacion).ConfigureAwait(false));
        }
        catch (HttpRequestException)
        {
            return Resultado.Fallo<T>(new ErrorOperacion(TipoError.Conexion, Textos.ErrorConexion));
        }
        catch (TaskCanceledException) when (!cancelacion.IsCancellationRequested)
        {
            return Resultado.Fallo<T>(new ErrorOperacion(TipoError.Conexion, Textos.ErrorTiempoAgotado));
        }
        catch (JsonException)
        {
            return Resultado.Fallo<T>(new ErrorOperacion(TipoError.Desconocido, Textos.ErrorRespuestaInvalida));
        }
    }

    private static async Task<ErrorOperacion> TraducirErrorAsync(HttpResponseMessage respuesta, CancellationToken cancelacion)
    {
        var tipo = respuesta.StatusCode switch
        {
            HttpStatusCode.BadRequest => TipoError.Validacion,
            HttpStatusCode.Unauthorized => TipoError.NoAutenticado,
            HttpStatusCode.Forbidden => TipoError.Prohibido,
            HttpStatusCode.NotFound => TipoError.NoEncontrado,
            HttpStatusCode.Conflict => TipoError.Conflicto,
            HttpStatusCode.ServiceUnavailable => TipoError.ServicioNoDisponible,
            _ => TipoError.Desconocido
        };
        var mensaje = await LeerMensajeAsync(respuesta, cancelacion).ConfigureAwait(false);
        return new ErrorOperacion(tipo, mensaje ?? Textos.ErrorServidor);
    }

    private static async Task<string?> LeerMensajeAsync(HttpResponseMessage respuesta, CancellationToken cancelacion)
    {
        if (respuesta.Content.Headers.ContentType?.MediaType != "application/json")
        {
            return null;
        }
        try
        {
            var cuerpo = await respuesta.Content.ReadFromJsonAsync<RespuestaError>(OpcionesJson, cancelacion).ConfigureAwait(false);
            return string.IsNullOrWhiteSpace(cuerpo?.Error) ? null : cuerpo.Error;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed record RespuestaError(string? Error);
}

public sealed class ManejadorAutenticacion(ISesionUsuario sesion) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!string.IsNullOrEmpty(sesion.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
