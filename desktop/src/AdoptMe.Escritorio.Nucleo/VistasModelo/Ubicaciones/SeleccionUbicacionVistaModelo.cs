using System.Globalization;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;

public sealed partial class SeleccionUbicacionVistaModelo(IServicioGeolocalizacion geolocalizacion, IDialogos dialogos)
    : VistaModeloDialogo<Ubicacion>, IAlNavegar
{
    public static Coordenadas CentroMexico { get; } = new(23.6345, -102.5528);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Descripcion))]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    private Ubicacion? seleccionada;

    public string Descripcion => Seleccionada is null ? string.Empty : Describir(Seleccionada);

    public static string Describir(Ubicacion ubicacion)
    {
        ArgumentNullException.ThrowIfNull(ubicacion);
        return string.Create(CultureInfo.CurrentCulture,
            $"Ciudad: {ubicacion.Ciudad}\nEstado: {ubicacion.Estado}\nPaís: {ubicacion.Pais}\nLatitud: {ubicacion.Latitud:F6}\nLongitud: {ubicacion.Longitud:F6}");
    }

    public override void Inicializar(object? parametro) => Seleccionada = parametro as Ubicacion;

    public async Task AlNavegarAsync(object? parametro)
    {
        if (Seleccionada is not null)
        {
            return;
        }
        var aproximada = await MientrasOcupadoAsync(geolocalizacion.ObtenerUbicacionAproximadaAsync);
        if (aproximada.Exito)
        {
            Seleccionada = aproximada.Valor;
        }
        else
        {
            dialogos.Mostrar(aproximada.Error.Mensaje, TipoMensaje.Advertencia, Textos.Advertencia);
        }
    }

    [RelayCommand]
    private async Task SeleccionarPuntoAsync(Coordenadas punto)
    {
        var direccion = await MientrasOcupadoAsync(() => geolocalizacion.ObtenerDireccionAsync(punto));
        if (!direccion.Exito)
        {
            dialogos.Mostrar(direccion.Error.Mensaje, TipoMensaje.Error, Textos.Error);
            return;
        }
        if (!EsDeMexico(direccion.Valor))
        {
            dialogos.Mostrar(Textos.SoloMexico, TipoMensaje.Advertencia, Textos.Advertencia);
            return;
        }
        Seleccionada = direccion.Valor;
    }

    private bool PuedeGuardar() => Seleccionada is not null;

    [RelayCommand(CanExecute = nameof(PuedeGuardar))]
    private void Guardar()
    {
        if (Seleccionada is not null)
        {
            Aceptar(Seleccionada);
        }
    }

    internal static bool EsDeMexico(Ubicacion ubicacion) =>
        ubicacion.Pais is { } pais && (pais.Contains("México", StringComparison.OrdinalIgnoreCase) || pais.Contains("Mexico", StringComparison.OrdinalIgnoreCase));
}
