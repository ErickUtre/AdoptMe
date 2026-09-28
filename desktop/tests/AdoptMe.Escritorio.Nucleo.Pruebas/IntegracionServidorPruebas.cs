using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.Servicios.TiempoReal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public sealed class HechoIntegracionAttribute : FactAttribute
{
    public const string Variable = "ADOPTME_SERVIDOR";

    public HechoIntegracionAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(Variable)))
        {
            Skip = $"Define {Variable} (por ejemplo http://localhost:8080) para ejecutar las pruebas contra el servidor.";
        }
    }
}

public sealed class IntegracionServidorPruebas
{
    private static readonly Ubicacion Xalapa = new() { Latitud = 19.54162, Longitud = -96.932527, Ciudad = "Xalapa", Estado = "Veracruz", Pais = "México" };
    private static readonly Ubicacion CercaDeXalapa = new() { Latitud = 19.54562, Longitud = -96.930527, Ciudad = "Xalapa", Estado = "Veracruz", Pais = "México" };

    private sealed class Cliente : IAsyncDisposable
    {
        public Cliente(IOptions<OpcionesServidor> opciones)
        {
            var http = new HttpClient(new ManejadorAutenticacion(Sesion) { InnerHandler = new HttpClientHandler() })
            {
                BaseAddress = opciones.Value.UrlApi
            };
            var api = new ClienteApi(http);
            Cuentas = new ServicioCuentas(api);
            Adopciones = new ServicioAdopciones(api);
            Solicitudes = new ServicioSolicitudes(api);
            Chat = new ServicioChat(api);
            Grpc = new CanalGrpc(opciones, Sesion);
            Mapa = new ServicioMapa(Grpc);
            Archivos = new ServicioArchivos(Grpc);
            Notificaciones = new CanalNotificaciones(Grpc, NullLogger<CanalNotificaciones>.Instance);
            CanalChat = new CanalChat(opciones, Sesion);
        }

        public SesionUsuario Sesion { get; } = new();

        public ServicioCuentas Cuentas { get; }

        public ServicioAdopciones Adopciones { get; }

        public ServicioSolicitudes Solicitudes { get; }

        public ServicioChat Chat { get; }

        public CanalGrpc Grpc { get; }

        public ServicioMapa Mapa { get; }

        public ServicioArchivos Archivos { get; }

        public CanalNotificaciones Notificaciones { get; }

        public CanalChat CanalChat { get; }

        public async Task RegistrarYEntrarAsync(string nombre, Ubicacion ubicacion)
        {
            var correo = $"{nombre.ToLowerInvariant()}.{Guid.NewGuid():N}@pruebas.com";
            Assert.True((await Cuentas.RegistrarAsync(new SolicitudRegistro(nombre, "2281234567", correo, "Segura123", ubicacion))).Exito);
            var sesion = await Cuentas.IniciarSesionAsync(correo, "Segura123");
            Assert.True(sesion.Exito, sesion.Error?.Mensaje);
            Sesion.Iniciar(sesion.Valor);
        }

        public async ValueTask DisposeAsync()
        {
            Notificaciones.Dispose();
            await CanalChat.DisposeAsync();
            Grpc.Dispose();
        }
    }

    private static IOptions<OpcionesServidor> Opciones()
    {
        var baseUrl = new Uri(Environment.GetEnvironmentVariable(HechoIntegracionAttribute.Variable)!);
        var grpc = Environment.GetEnvironmentVariable("ADOPTME_GRPC") ?? $"http://{baseUrl.Host}:50051";
        return Options.Create(new OpcionesServidor
        {
            UrlApi = new Uri(baseUrl, "api/"),
            UrlTiempoReal = baseUrl,
            UrlGrpc = new Uri(grpc)
        });
    }

    [HechoIntegracion]
    public async Task RecorreElFlujoCompletoDeUnaAdopcion()
    {
        var opciones = Opciones();
        await using var publicador = new Cliente(opciones);
        await using var interesado = new Cliente(opciones);
        await publicador.RegistrarYEntrarAsync("Ana", Xalapa);
        await interesado.RegistrarYEntrarAsync("Luis", CercaDeXalapa);

        var notificacionRecibida = new TaskCompletionSource<Notificacion>(TaskCreationOptions.RunContinuationsAsynchronously);
        interesado.Notificaciones.NotificacionRecibida += (_, notificacion) => notificacionRecibida.TrySetResult(notificacion);
        interesado.Notificaciones.Iniciar();
        await Task.Delay(1000);

        var mascota = new Mascota { Nombre = "Max", Especie = "Perro", Raza = "Mestizo", Edad = "2 año(s) con 0 mes(es)", Sexo = SexosMascota.Macho, Tamano = "Mediano", Descripcion = "Juguetón" };
        var registro = await publicador.Adopciones.RegistrarAsync(mascota, Xalapa);
        Assert.True(registro.Exito, registro.Error?.Mensaje);

        var notificacion = await notificacionRecibida.Task.WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(TiposNotificacion.AdopcionCercana, notificacion.Tipo);
        Assert.Equal(registro.Valor.AdopcionID, notificacion.ReferenciaID);

        var cercanas = await interesado.Mapa.ObtenerCercanasAsync(CercaDeXalapa.Coordenadas);
        Assert.True(cercanas.Exito, cercanas.Error?.Mensaje);
        var cercana = Assert.Single(cercanas.Valor, adopcion => adopcion.AdopcionId == registro.Valor.AdopcionID);
        Assert.Equal("Mediano", cercana.Mascota.Tamano);

        var foto = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");
        await File.WriteAllBytesAsync(foto, [1, 2, 3, 4, 5]);
        try
        {
            Assert.True((await publicador.Archivos.SubirFotoMascotaAsync(registro.Valor.MascotaID, foto)).Exito);
            var ajena = await interesado.Archivos.SubirFotoMascotaAsync(registro.Valor.MascotaID, foto);
            Assert.Equal(Comun.TipoError.Prohibido, ajena.Error?.Tipo);
        }
        finally
        {
            File.Delete(foto);
        }
        var descargada = await interesado.Adopciones.ObtenerFotoMascotaAsync(registro.Valor.MascotaID);
        Assert.Equal([1, 2, 3, 4, 5], descargada.Valor);

        Assert.True((await interesado.Solicitudes.RegistrarAsync(registro.Valor.AdopcionID)).Exito);
        var solicitudes = await publicador.Solicitudes.ListarAsync(registro.Valor.AdopcionID);
        var solicitud = Assert.Single(solicitudes.Valor!);
        Assert.Equal("Luis", solicitud.NombreAdoptante);
        Assert.True((await publicador.Solicitudes.AceptarAsync(registro.Valor.AdopcionID, solicitud.SolicitudID)).Exito);

        var propias = await publicador.Adopciones.ListarPropiasAsync();
        Assert.True(Assert.Single(propias.Valor!).Estado);
        Assert.True((await publicador.Adopciones.EliminarAsync(registro.Valor.AdopcionID)).Exito);
    }

    [HechoIntegracion]
    public async Task EntregaMensajesDeChatEnTiempoReal()
    {
        var opciones = Opciones();
        await using var ana = new Cliente(opciones);
        await using var luis = new Cliente(opciones);
        await ana.RegistrarYEntrarAsync("Ana", Xalapa);
        await luis.RegistrarYEntrarAsync("Luis", CercaDeXalapa);

        var recibido = new TaskCompletionSource<Mensaje>(TaskCreationOptions.RunContinuationsAsynchronously);
        ana.CanalChat.MensajeRecibido += (_, mensaje) => recibido.TrySetResult(mensaje);
        await ana.CanalChat.ConectarAsync();

        await luis.CanalChat.EnviarAsync(ana.Sesion.UsuarioId, "Hola, me interesa Max");

        var mensaje = await recibido.Task.WaitAsync(TimeSpan.FromSeconds(15));
        Assert.Equal(luis.Sesion.UsuarioId, mensaje.RemitenteID);
        Assert.Equal("Hola, me interesa Max", mensaje.Contenido);

        var conversaciones = await ana.Chat.ListarConversacionesAsync();
        Assert.Equal("Luis", Assert.Single(conversaciones.Valor!).Nombre);
    }
}
