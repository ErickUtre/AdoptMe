using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AdoptMe.Escritorio.Controles;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using GMap.NET.WindowsPresentation;

namespace AdoptMe.Escritorio.Vistas.Dialogos;

public partial class SeleccionUbicacionVentana : Window
{
    private const double ZoomSeleccion = 15;
    private const double ZoomPais = 5;
    private const double TamanoMarcador = 32;

    private GMapMarker? marcador;

    public SeleccionUbicacionVentana()
    {
        InitializeComponent();
        Mapas.Configurar(Mapa);
        Mapa.Position = Mapas.APunto(SeleccionUbicacionVistaModelo.CentroMexico);
        Mapa.Zoom = ZoomPais;
        DataContextChanged += AlCambiarContexto;
        Closed += (_, _) => Mapa.Dispose();
    }

    private SeleccionUbicacionVistaModelo? VistaModelo => DataContext as SeleccionUbicacionVistaModelo;

    private void AlCambiarContexto(object remitente, DependencyPropertyChangedEventArgs argumentos)
    {
        if (argumentos.OldValue is INotifyPropertyChanged anterior)
        {
            anterior.PropertyChanged -= AlCambiarPropiedad;
        }
        if (argumentos.NewValue is INotifyPropertyChanged nuevo)
        {
            nuevo.PropertyChanged += AlCambiarPropiedad;
        }
        MostrarSeleccion();
    }

    private void AlCambiarPropiedad(object? remitente, PropertyChangedEventArgs argumentos)
    {
        if (argumentos.PropertyName == nameof(SeleccionUbicacionVistaModelo.Seleccionada))
        {
            MostrarSeleccion();
        }
    }

    private void MostrarSeleccion()
    {
        if (VistaModelo?.Seleccionada is not { } seleccionada)
        {
            return;
        }
        if (marcador is not null)
        {
            Mapa.Markers.Remove(marcador);
        }
        marcador = Mapas.CrearMarcador(seleccionada.Coordenadas, Mapas.IconoUbicacion, TamanoMarcador);
        Mapa.Markers.Add(marcador);
        Mapa.Position = marcador.Position;
        Mapa.Zoom = Math.Max(Mapa.Zoom, ZoomSeleccion);
    }

    private void AlHacerDobleClic(object remitente, MouseButtonEventArgs argumentos)
    {
        var punto = argumentos.GetPosition(Mapa);
        var coordenadas = Mapas.ACoordenadas(Mapa.FromLocalToLatLng((int)punto.X, (int)punto.Y));
        argumentos.Handled = true;
        VistaModelo?.SeleccionarPuntoCommand.Execute(coordenadas);
    }
}
