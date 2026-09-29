using System.Net;
using System.Text;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class ServicioGeolocalizacionPruebas
{
    private const string RespuestaIp = """{"lat":19.43,"lon":-99.13,"city":"Ciudad de México","regionName":"CDMX","country":"México"}""";
    private const string RespuestaDireccion = """{"address":{"town":"Xalapa","state":"Veracruz","country":"México"}}""";

    private static readonly Coordenadas PosicionDispositivo = new(19.5438, -96.9102);

    private static ServicioGeolocalizacion CrearServicio(Resultado<Coordenadas> posicion, Func<HttpRequestMessage, HttpResponseMessage> responder) =>
        new(new HttpClient(new ManejadorHttpFalso(responder)),
            new ProveedorPosicionFalso(posicion),
            Options.Create(new OpcionesGeolocalizacion()),
            NullLogger<ServicioGeolocalizacion>.Instance);

    private static HttpResponseMessage Json(string contenido) =>
        new(HttpStatusCode.OK) { Content = new StringContent(contenido, Encoding.UTF8, "application/json") };

    private static HttpResponseMessage SegunServicio(HttpRequestMessage peticion) =>
        Json(peticion.RequestUri!.Host.Contains("nominatim", StringComparison.Ordinal) ? RespuestaDireccion : RespuestaIp);

    private static Resultado<Coordenadas> SinPosicion() =>
        Resultado.Fallo<Coordenadas>(new ErrorOperacion(TipoError.Prohibido, "Denied"));

    [Fact]
    public async Task UsaLaPosicionDelDispositivoYCompletaLaDireccion()
    {
        var servicio = CrearServicio(Resultado.Correcto(PosicionDispositivo), SegunServicio);

        var resultado = await servicio.ObtenerUbicacionActualAsync();

        Assert.True(resultado.Exito);
        Assert.True(resultado.Valor.EsPrecisa);
        Assert.Equal(PosicionDispositivo, resultado.Valor.Ubicacion.Coordenadas);
        Assert.Equal("Xalapa", resultado.Valor.Ubicacion.Ciudad);
    }

    [Fact]
    public async Task ConservaLaPosicionPrecisaAunqueFalleLaDireccion()
    {
        var servicio = CrearServicio(Resultado.Correcto(PosicionDispositivo), _ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var resultado = await servicio.ObtenerUbicacionActualAsync();

        Assert.True(resultado.Exito);
        Assert.True(resultado.Valor.EsPrecisa);
        Assert.Equal(PosicionDispositivo, resultado.Valor.Ubicacion.Coordenadas);
        Assert.Null(resultado.Valor.Ubicacion.Ciudad);
    }

    [Fact]
    public async Task RecurreALaUbicacionPorIpSiElDispositivoNoLaProporciona()
    {
        var servicio = CrearServicio(SinPosicion(), SegunServicio);

        var resultado = await servicio.ObtenerUbicacionActualAsync();

        Assert.True(resultado.Exito);
        Assert.False(resultado.Valor.EsPrecisa);
        Assert.Equal("Ciudad de México", resultado.Valor.Ubicacion.Ciudad);
    }

    [Fact]
    public async Task FallaCuandoNingunaFuenteResponde()
    {
        var servicio = CrearServicio(SinPosicion(), _ => throw new HttpRequestException("sin red"));

        var resultado = await servicio.ObtenerUbicacionActualAsync();

        Assert.False(resultado.Exito);
        Assert.Equal(TipoError.Conexion, resultado.Error.Tipo);
    }

    [Fact]
    public async Task ElSelectorAdvierteCuandoLaUbicacionEsAproximada()
    {
        var dialogos = new DialogosFalsos();
        var vistaModelo = new SeleccionUbicacionVistaModelo(CrearServicio(SinPosicion(), SegunServicio), dialogos);

        await vistaModelo.AlNavegarAsync(null);

        Assert.Equal("Ciudad de México", vistaModelo.Seleccionada?.Ciudad);
        Assert.Contains((Textos.UbicacionAproximada, TipoMensaje.Advertencia), dialogos.Mensajes);
    }

    [Fact]
    public async Task ElSelectorNoAdvierteCuandoLaUbicacionEsPrecisa()
    {
        var dialogos = new DialogosFalsos();
        var vistaModelo = new SeleccionUbicacionVistaModelo(CrearServicio(Resultado.Correcto(PosicionDispositivo), SegunServicio), dialogos);

        await vistaModelo.AlNavegarAsync(null);

        Assert.Equal(PosicionDispositivo, vistaModelo.Seleccionada?.Coordenadas);
        Assert.Empty(dialogos.Mensajes);
    }

    private sealed class ProveedorPosicionFalso(Resultado<Coordenadas> posicion) : IProveedorPosicion
    {
        public Task<Resultado<Coordenadas>> ObtenerPosicionAsync(CancellationToken cancelacion = default) => Task.FromResult(posicion);
    }
}
