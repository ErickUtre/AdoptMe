using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.Servicios.TiempoReal;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Notificaciones;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Perfil;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Principal;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Reportes;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Solicitudes;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AdoptMe.Escritorio.Nucleo;

public static class ExtensionesServicios
{
    public static IServiceCollection AgregarNucleoAdoptMe(this IServiceCollection servicios, IConfiguration configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        servicios.AddOptions<OpcionesServidor>().Bind(configuracion.GetSection(OpcionesServidor.Seccion)).ValidateDataAnnotations().ValidateOnStart();
        servicios.AddOptions<OpcionesGeolocalizacion>().Bind(configuracion.GetSection(OpcionesGeolocalizacion.Seccion)).ValidateDataAnnotations().ValidateOnStart();

        servicios.AddSingleton<ISesionUsuario, SesionUsuario>();
        servicios.AddSingleton<INavegador, Navegador>();
        servicios.AddTransient<ManejadorAutenticacion>();

        servicios.AddHttpClient<ClienteApi>((proveedor, http) =>
        {
            var opciones = proveedor.GetRequiredService<IOptions<OpcionesServidor>>().Value;
            http.BaseAddress = opciones.UrlApi;
            http.Timeout = opciones.TiempoEsperaPeticiones;
        }).AddHttpMessageHandler<ManejadorAutenticacion>();
        servicios.AddHttpClient<IServicioGeolocalizacion, ServicioGeolocalizacion>();

        servicios.AddTransient<IServicioCuentas, ServicioCuentas>();
        servicios.AddTransient<IServicioAdopciones, ServicioAdopciones>();
        servicios.AddTransient<IServicioSolicitudes, ServicioSolicitudes>();
        servicios.AddTransient<IServicioChat, ServicioChat>();
        servicios.AddTransient<IServicioNotificaciones, ServicioNotificaciones>();

        servicios.AddSingleton<CanalGrpc>();
        servicios.AddSingleton<IServicioMapa, ServicioMapa>();
        servicios.AddSingleton<IServicioArchivos, ServicioArchivos>();
        servicios.AddSingleton<ICanalNotificaciones, CanalNotificaciones>();
        servicios.AddSingleton<ICanalChat, CanalChat>();

        return servicios.AgregarVistasModelo();
    }

    private static IServiceCollection AgregarVistasModelo(this IServiceCollection servicios) => servicios
        .AddTransient<InicioSesionVistaModelo>()
        .AddTransient<RegistroUsuarioVistaModelo>()
        .AddTransient<MenuPrincipalVistaModelo>()
        .AddTransient<MapaAdopcionesVistaModelo>()
        .AddTransient<DetalleAdopcionVistaModelo>()
        .AddTransient<AdopcionesPropiasVistaModelo>()
        .AddTransient<EdicionAdopcionVistaModelo>()
        .AddTransient<RegistroAdopcionVistaModelo>()
        .AddTransient<SolicitudesVistaModelo>()
        .AddTransient<EditarCampoVistaModelo>()
        .AddTransient<SeleccionUbicacionVistaModelo>()
        .AddTransient<PerfilVistaModelo>()
        .AddTransient<ConversacionesVistaModelo>()
        .AddTransient<ChatVistaModelo>()
        .AddTransient<NotificacionesVistaModelo>()
        .AddTransient<ReportesVistaModelo>()
        .AddTransient<ReporteVistaModelo>();
}
