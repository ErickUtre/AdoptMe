using System.Windows;
using System.Windows.Threading;
using AdoptMe.Escritorio.Infraestructura;
using AdoptMe.Escritorio.Nucleo;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Reportes;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Solicitudes;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using AdoptMe.Escritorio.Vistas.Dialogos;
using AdoptMe.Escritorio.Vistas.Ventanas;
using GMap.NET;
using GMap.NET.MapProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdoptMe.Escritorio;

public partial class App : Application
{
    private IHost? anfitrion;
    private bool cerrandoPorError;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += AlOcurrirErrorNoControlado;

        anfitrion = Host.CreateDefaultBuilder(e.Args)
            .ConfigureAppConfiguration(configuracion => configuracion.SetBasePath(AppContext.BaseDirectory))
            .ConfigureServices((contexto, servicios) => RegistrarServicios(servicios, contexto.Configuration))
            .Build();
        await anfitrion.StartAsync().ConfigureAwait(true);

        var opcionesGeolocalizacion = anfitrion.Services.GetRequiredService<IConfiguration>().GetSection("Geolocalizacion");
        GMapProvider.UserAgent = opcionesGeolocalizacion["AgenteUsuario"] ?? "AdoptMe-Escritorio";
        GMaps.Instance.Mode = AccessMode.ServerAndCache;

        anfitrion.Services.GetRequiredService<IVentanas>().MostrarInicioSesion();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (anfitrion is not null)
        {
            await anfitrion.StopAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(true);
            anfitrion.Dispose();
        }
        base.OnExit(e);
    }

    private static void RegistrarServicios(IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AgregarNucleoAdoptMe(configuracion);
        servicios.AddSingleton<IDespachadorUi, DespachadorWpf>();
        servicios.AddSingleton<IDialogos, Dialogos>();
        servicios.AddSingleton<IVentanas, Ventanas>();
        servicios.AddSingleton<INotificadorEmergente, NotificadorEmergente>();
        servicios.AddSingleton<IExportadorReportes, ExportadorReportesPdf>();
        servicios.AddSingleton(new RegistroDialogos()
            .Registrar<RegistroUsuarioVistaModelo>(() => new RegistroUsuarioVentana())
            .Registrar<SeleccionUbicacionVistaModelo>(() => new SeleccionUbicacionVentana())
            .Registrar<EditarCampoVistaModelo>(() => new EditarCampoVentana())
            .Registrar<SolicitudesVistaModelo>(() => new SolicitudesVentana()));
    }

    private void AlOcurrirErrorNoControlado(object remitente, DispatcherUnhandledExceptionEventArgs argumentos)
    {
        argumentos.Handled = true;
        if (anfitrion is not null)
        {
            RegistrarErrorNoControlado(anfitrion.Services.GetRequiredService<ILogger<App>>(), argumentos.Exception);
        }
        if (cerrandoPorError)
        {
            return;
        }
        cerrandoPorError = true;
        foreach (Window ventana in Windows)
        {
            ventana.Hide();
        }
        MessageBox.Show(Textos.ErrorInesperado, Textos.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        Shutdown(1);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Error no controlado en la interfaz")]
    private static partial void RegistrarErrorNoControlado(ILogger registro, Exception excepcion);
}
