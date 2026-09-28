using System.Net;
using System.Text;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class ClienteApiPruebas
{
    private static ClienteApi CrearCliente(Func<HttpRequestMessage, HttpResponseMessage> responder) =>
        new(new HttpClient(new ManejadorHttpFalso(responder)) { BaseAddress = new Uri("http://servidor/api/") });

    private static HttpResponseMessage Json(HttpStatusCode codigo, string contenido) =>
        new(codigo) { Content = new StringContent(contenido, Encoding.UTF8, "application/json") };

    [Fact]
    public async Task DeserializaLaRespuestaExitosa()
    {
        var cliente = CrearCliente(_ => Json(HttpStatusCode.OK, """{"UsuarioID":3,"Nombre":"Luis"}"""));

        var resultado = await cliente.ObtenerAsync<ResumenUsuario>("usuarios/3");

        Assert.True(resultado.Exito);
        Assert.Equal(new ResumenUsuario(3, "Luis"), resultado.Valor);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, TipoError.Validacion)]
    [InlineData(HttpStatusCode.Unauthorized, TipoError.NoAutenticado)]
    [InlineData(HttpStatusCode.Forbidden, TipoError.Prohibido)]
    [InlineData(HttpStatusCode.NotFound, TipoError.NoEncontrado)]
    [InlineData(HttpStatusCode.Conflict, TipoError.Conflicto)]
    [InlineData(HttpStatusCode.ServiceUnavailable, TipoError.ServicioNoDisponible)]
    public async Task TraduceLosCodigosDeErrorConElMensajeDelServidor(HttpStatusCode codigo, TipoError esperado)
    {
        var cliente = CrearCliente(_ => Json(codigo, """{"error":"mensaje del servidor"}"""));

        var resultado = await cliente.EnviarAsync(HttpMethod.Post, "adopciones", new { });

        Assert.False(resultado.Exito);
        Assert.Equal(new ErrorOperacion(esperado, "mensaje del servidor"), resultado.Error);
    }

    [Fact]
    public async Task InformaErroresDeConexion()
    {
        var cliente = CrearCliente(_ => throw new HttpRequestException("sin red"));

        var resultado = await cliente.ObtenerListaAsync<Notificacion>("notificaciones");

        Assert.Equal(TipoError.Conexion, resultado.Error?.Tipo);
    }

    [Fact]
    public async Task EnviaSoloLosCamposPresentesYElTokenDeLaSesion()
    {
        var sesion = new SesionUsuario();
        sesion.Iniciar(new Modelos.Sesion("abc", false, new Perfil { UsuarioID = 1 }));
        HttpRequestMessage? peticion = null;
        string? cuerpo = null;
        using var manejador = new ManejadorAutenticacion(sesion)
        {
            InnerHandler = new ManejadorHttpFalso(mensaje =>
            {
                peticion = mensaje;
                cuerpo = mensaje.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return Json(HttpStatusCode.OK, """{"UsuarioID":1,"Nombre":"Ana"}""");
            })
        };
        var servicio = new ServicioCuentas(new ClienteApi(new HttpClient(manejador) { BaseAddress = new Uri("http://servidor/api/") }));

        await servicio.ActualizarPerfilAsync("Ana", null);

        Assert.Equal("Bearer abc", peticion?.Headers.Authorization?.ToString());
        Assert.Equal(HttpMethod.Patch, peticion?.Method);
        Assert.Equal("""{"Nombre":"Ana"}""", cuerpo);
    }
}
