using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.VistasModelo;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

internal sealed class DialogosFalsos : IDialogos
{
    public List<(string Mensaje, TipoMensaje Tipo)> Mensajes { get; } = [];

    public bool RespuestaConfirmacion { get; set; } = true;

    public object? ResultadoDialogo { get; set; }

    public void Mostrar(string mensaje, TipoMensaje tipo = TipoMensaje.Informacion, string? titulo = null) => Mensajes.Add((mensaje, tipo));

    public bool Confirmar(string mensaje, string? titulo = null) => RespuestaConfirmacion;

    public string? SeleccionarArchivo(FiltroArchivo filtro) => null;

    public string? SeleccionarDestino(FiltroArchivo filtro, string nombreSugerido) => null;

    public TResultado? Abrir<TVistaModelo, TResultado>(object? parametro = null)
        where TVistaModelo : VistaModeloDialogo<TResultado> => (TResultado?)ResultadoDialogo;

    public void MostrarImagen(byte[] imagen)
    {
    }

    public void ReproducirVideo(byte[] video)
    {
    }
}

internal sealed class VentanasFalsas : IVentanas
{
    public int VecesPrincipal { get; private set; }

    public void MostrarInicioSesion()
    {
    }

    public void MostrarPrincipal() => VecesPrincipal++;
}

internal sealed class CuentasFalsas : IServicioCuentas
{
    public Resultado<Modelos.Sesion> RespuestaInicioSesion { get; set; } =
        Resultado.Fallo<Modelos.Sesion>(new ErrorOperacion(TipoError.Desconocido, "sin configurar"));

    public Resultado RespuestaRegistro { get; set; } = Resultado.Correcto();

    public SolicitudRegistro? UltimoRegistro { get; private set; }

    public Task<Resultado<Modelos.Sesion>> IniciarSesionAsync(string correo, string contrasena) => Task.FromResult(RespuestaInicioSesion);

    public Task<Resultado> RegistrarAsync(SolicitudRegistro solicitud)
    {
        UltimoRegistro = solicitud;
        return Task.FromResult(RespuestaRegistro);
    }

    public Task<Resultado<ResumenUsuario>> ObtenerResumenAsync(int usuarioId) =>
        Task.FromResult(Resultado.Correcto(new ResumenUsuario(usuarioId, "Contraparte")));

    public Task<Resultado<Perfil>> ActualizarPerfilAsync(string? nombre, string? telefono) => throw new NotSupportedException();

    public Task<Resultado> ActualizarCorreoAsync(string correo) => throw new NotSupportedException();

    public Task<Resultado<Ubicacion>> ActualizarUbicacionAsync(Ubicacion ubicacion) => throw new NotSupportedException();

    public Task<Resultado<byte[]>> ObtenerFotoPerfilAsync() => throw new NotSupportedException();
}

internal sealed class ManejadorHttpFalso(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(responder(request));
}
