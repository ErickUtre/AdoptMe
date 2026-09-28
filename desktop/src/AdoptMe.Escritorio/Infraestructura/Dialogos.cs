using System.IO;
using System.Windows;
using AdoptMe.Escritorio.Nucleo;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.VistasModelo;
using AdoptMe.Escritorio.Vistas.Dialogos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace AdoptMe.Escritorio.Infraestructura;

public sealed class Dialogos(IServiceProvider proveedor, RegistroDialogos registro) : IDialogos
{
    public void Mostrar(string mensaje, TipoMensaje tipo = TipoMensaje.Informacion, string? titulo = null)
    {
        var icono = tipo switch
        {
            TipoMensaje.Error => MessageBoxImage.Error,
            TipoMensaje.Advertencia => MessageBoxImage.Warning,
            _ => MessageBoxImage.Information
        };
        MessageBox.Show(VentanaActiva(), mensaje, titulo ?? Textos.Aplicacion, MessageBoxButton.OK, icono);
    }

    public bool Confirmar(string mensaje, string? titulo = null) =>
        MessageBox.Show(VentanaActiva(), mensaje, titulo ?? Textos.Confirmacion, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public string? SeleccionarArchivo(FiltroArchivo filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        var dialogo = new OpenFileDialog { Filter = CrearFiltro(filtro), Multiselect = false, CheckFileExists = true };
        return dialogo.ShowDialog(VentanaActiva()) == true ? dialogo.FileName : null;
    }

    public string? SeleccionarDestino(FiltroArchivo filtro, string nombreSugerido)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        var dialogo = new SaveFileDialog { Filter = CrearFiltro(filtro), FileName = nombreSugerido };
        return dialogo.ShowDialog(VentanaActiva()) == true ? dialogo.FileName : null;
    }

    public TResultado? Abrir<TVistaModelo, TResultado>(object? parametro = null)
        where TVistaModelo : VistaModeloDialogo<TResultado>
    {
        var vistaModelo = proveedor.GetRequiredService<TVistaModelo>();
        vistaModelo.Inicializar(parametro);

        var ventana = registro.Crear(typeof(TVistaModelo));
        ventana.DataContext = vistaModelo;
        ventana.Owner = VentanaActiva();
        vistaModelo.CierreSolicitado += (_, aceptado) =>
        {
            if (ventana.IsVisible && ventana.DialogResult is null)
            {
                ventana.DialogResult = aceptado;
            }
        };
        if (vistaModelo is IAlNavegar alNavegar)
        {
            ventana.Loaded += async (_, _) => await alNavegar.AlNavegarAsync(parametro).ConfigureAwait(true);
        }

        return ventana.ShowDialog() == true ? vistaModelo.Resultado : default;
    }

    public void MostrarImagen(byte[] imagen) =>
        new ImagenVentana(imagen) { Owner = VentanaActiva() }.ShowDialog();

    public void ReproducirVideo(byte[] video)
    {
        ArgumentNullException.ThrowIfNull(video);
        var ruta = Path.Combine(Path.GetTempPath(), $"adoptme-{Guid.NewGuid():N}.mp4");
        File.WriteAllBytes(ruta, video);
        try
        {
            new VideoVentana(new Uri(ruta)) { Owner = VentanaActiva() }.ShowDialog();
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    private static string CrearFiltro(FiltroArchivo filtro)
    {
        var patrones = string.Join(';', filtro.Extensiones.Select(extension => $"*{extension}"));
        return $"{filtro.Descripcion} ({patrones})|{patrones}";
    }

    private static Window? VentanaActiva() =>
        Application.Current.Windows.OfType<Window>().FirstOrDefault(ventana => ventana.IsActive) ?? Application.Current.MainWindow;
}

public sealed class RegistroDialogos
{
    private readonly Dictionary<Type, Func<Window>> fabricas = [];

    public RegistroDialogos Registrar<TVistaModelo>(Func<Window> fabrica)
    {
        fabricas[typeof(TVistaModelo)] = fabrica;
        return this;
    }

    public Window Crear(Type tipoVistaModelo) =>
        fabricas.TryGetValue(tipoVistaModelo, out var fabrica)
            ? fabrica()
            : throw new InvalidOperationException($"No hay una ventana registrada para {tipoVistaModelo.Name}.");
}
