using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Reportes;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Principal;
using AdoptMe.Escritorio.Vistas.Ventanas;
using Microsoft.Extensions.DependencyInjection;
using OxyPlot;
using ExportadorPdfSkia = OxyPlot.SkiaSharp.PdfExporter;

namespace AdoptMe.Escritorio.Infraestructura;

public sealed class DespachadorWpf : IDespachadorUi
{
    private readonly Dispatcher despachador = Application.Current.Dispatcher;

    public void Ejecutar(Action accion)
    {
        if (despachador.CheckAccess())
        {
            accion();
            return;
        }
        despachador.BeginInvoke(accion);
    }
}

public sealed class Ventanas(IServiceProvider proveedor) : IVentanas
{
    public void MostrarInicioSesion() =>
        Reemplazar(new InicioSesionVentana { DataContext = proveedor.GetRequiredService<InicioSesionVistaModelo>() });

    public void MostrarPrincipal()
    {
        var vistaModelo = proveedor.GetRequiredService<MenuPrincipalVistaModelo>();
        var ventana = new MenuPrincipalVentana { DataContext = vistaModelo };
        ventana.Loaded += async (_, _) => await vistaModelo.IniciarAsync().ConfigureAwait(true);
        Reemplazar(ventana);
    }

    private static void Reemplazar(Window nueva)
    {
        var anterior = Application.Current.MainWindow;
        Application.Current.MainWindow = nueva;
        nueva.Show();
        if (anterior is not null && !ReferenceEquals(anterior, nueva))
        {
            anterior.Close();
        }
    }
}

public sealed class NotificadorEmergente : INotificadorEmergente
{
    private static readonly TimeSpan Duracion = TimeSpan.FromSeconds(4);

    public void Mostrar(string titulo, string mensaje)
    {
        var areaTrabajo = SystemParameters.WorkArea;
        var ventana = new Window
        {
            Width = 320,
            Height = 110,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            ResizeMode = ResizeMode.NoResize,
            Background = new SolidColorBrush(Color.FromRgb(0x20, 0x77, 0x66)),
            Foreground = Brushes.White,
            Topmost = true,
            ShowInTaskbar = false,
            ShowActivated = false,
            Content = new TextBlock
            {
                Text = $"{titulo}\n{mensaje}",
                FontSize = 14,
                Margin = new Thickness(14),
                TextWrapping = TextWrapping.Wrap
            }
        };
        ventana.Left = areaTrabajo.Right - ventana.Width - 12;
        ventana.Top = areaTrabajo.Bottom - ventana.Height - 12;
        ventana.Show();

        var temporizador = new DispatcherTimer { Interval = Duracion };
        temporizador.Tick += (_, _) =>
        {
            temporizador.Stop();
            ventana.Close();
        };
        temporizador.Start();
    }
}

public sealed class ExportadorReportesPdf : IExportadorReportes
{
    private const int Ancho = 800;
    private const int Alto = 600;

    public void ExportarPdf(PlotModel grafico, string ruta)
    {
        using var flujo = File.Create(ruta);
        new ExportadorPdfSkia { Width = Ancho, Height = Alto }.Export(grafico, flujo);
    }
}
