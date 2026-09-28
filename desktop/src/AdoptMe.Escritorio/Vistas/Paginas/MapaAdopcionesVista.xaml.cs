using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using AdoptMe.Escritorio.Controles;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using GMap.NET.WindowsPresentation;

namespace AdoptMe.Escritorio.Vistas.Paginas;

public partial class MapaAdopcionesVista : UserControl
{
    private const double TamanoMarcadorUsuario = 50;
    private const double TamanoMarcadorAdopcion = 44;

    private readonly DispatcherTimer esperaArrastre = new() { Interval = TimeSpan.FromMilliseconds(600) };
    private GMapMarker? marcadorUsuario;
    private MapaAdopcionesVistaModelo? vistaModelo;

    public MapaAdopcionesVista()
    {
        InitializeComponent();
        Mapas.Configurar(Mapa);
        Mapa.OnMapDrag += esperaArrastre.Start;
        Mapa.OnMapZoomChanged += ActualizarVisibilidadMarcadores;
        esperaArrastre.Tick += (_, _) => ConsultarCentro();
        DataContextChanged += AlCambiarContexto;
        Unloaded += (_, _) => Desvincular();
    }

    private void AlCambiarContexto(object remitente, DependencyPropertyChangedEventArgs argumentos)
    {
        Desvincular();
        vistaModelo = argumentos.NewValue as MapaAdopcionesVistaModelo;
        if (vistaModelo is null)
        {
            return;
        }
        vistaModelo.PropertyChanged += AlCambiarPropiedad;
        vistaModelo.Adopciones.CollectionChanged += AlCambiarAdopciones;
        Centrar();
        DibujarUsuario();
        DibujarAdopciones();
    }

    private void Desvincular()
    {
        esperaArrastre.Stop();
        if (vistaModelo is null)
        {
            return;
        }
        vistaModelo.PropertyChanged -= AlCambiarPropiedad;
        vistaModelo.Adopciones.CollectionChanged -= AlCambiarAdopciones;
    }

    private void AlCambiarPropiedad(object? remitente, PropertyChangedEventArgs argumentos)
    {
        switch (argumentos.PropertyName)
        {
            case nameof(MapaAdopcionesVistaModelo.Centro):
            case nameof(MapaAdopcionesVistaModelo.Zoom):
                Centrar();
                break;
            case nameof(MapaAdopcionesVistaModelo.UbicacionUsuario):
                DibujarUsuario();
                break;
        }
    }

    private void AlCambiarAdopciones(object? remitente, NotifyCollectionChangedEventArgs argumentos) => DibujarAdopciones();

    private void Centrar()
    {
        if (vistaModelo is null)
        {
            return;
        }
        Mapa.Position = Mapas.APunto(vistaModelo.Centro);
        Mapa.Zoom = vistaModelo.Zoom;
    }

    private void ConsultarCentro()
    {
        esperaArrastre.Stop();
        vistaModelo?.CargarCercanasCommand.Execute(Mapas.ACoordenadas(Mapa.Position));
    }

    private void DibujarUsuario()
    {
        if (marcadorUsuario is not null)
        {
            Mapa.Markers.Remove(marcadorUsuario);
        }
        if (vistaModelo?.UbicacionUsuario is not { } ubicacion)
        {
            return;
        }
        marcadorUsuario = Mapas.CrearMarcador(ubicacion, Mapas.IconoUbicacion, TamanoMarcadorUsuario);
        Mapa.Markers.Add(marcadorUsuario);
    }

    private void DibujarAdopciones()
    {
        foreach (var marcador in Mapa.Markers.Where(marcador => marcador.Tag is AdopcionCercana).ToList())
        {
            Mapa.Markers.Remove(marcador);
        }
        foreach (var adopcion in vistaModelo?.Adopciones ?? [])
        {
            Mapa.Markers.Add(CrearMarcadorAdopcion(adopcion));
        }
        ActualizarVisibilidadMarcadores();
    }

    private GMapMarker CrearMarcadorAdopcion(AdopcionCercana adopcion)
    {
        var marcador = Mapas.CrearMarcador(adopcion.Coordenadas, Mapas.IconoAdopcion, TamanoMarcadorAdopcion, adopcion);
        var tarjeta = new TarjetaMascota { DataContext = adopcion.Mascota };
        var emergente = new Popup
        {
            PlacementTarget = marcador.Shape,
            Placement = PlacementMode.Mouse,
            AllowsTransparency = true,
            StaysOpen = false,
            PopupAnimation = PopupAnimation.Fade,
            Child = tarjeta
        };
        marcador.Shape.MouseEnter += (_, _) => emergente.IsOpen = true;
        tarjeta.DetallesSolicitados += (_, _) =>
        {
            emergente.IsOpen = false;
            vistaModelo?.VerDetalleCommand.Execute(adopcion);
        };
        return marcador;
    }

    private void ActualizarVisibilidadMarcadores()
    {
        var visibilidad = Mapa.Zoom >= MapaAdopcionesVistaModelo.ZoomMinimoMarcadores ? Visibility.Visible : Visibility.Hidden;
        foreach (var marcador in Mapa.Markers.Where(marcador => marcador.Tag is AdopcionCercana))
        {
            marcador.Shape.Visibility = visibilidad;
        }
    }
}
