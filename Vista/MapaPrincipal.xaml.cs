using GMap.NET.MapProviders;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET.WindowsPresentation;
using System.Windows.Controls.Primitives;
using Cliente_AdoptMe.Utilidades;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MapaPrincipal.xaml
    /// </summary>
    public partial class MapaPrincipal : Page
    {
        private List<GMapMarker> marcadores = new List<GMapMarker>();
        private const int ZoomVisible = 10;

        public MapaPrincipal()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarMapaUbicacionDefecto();
        }
        
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            mapaPrincipal.Manager.CancelTileCaching();
            mapaPrincipal.Dispose();
            GMaps.Instance.CancelTileCaching();
        }
        
        private void CargarMapaUbicacionDefecto()
        {
            mapaPrincipal.MapProvider = GMapProviders.GoogleMap;
            var posicionDefectoMexico = new PointLatLng(23.6345, -102.5528);

            mapaPrincipal.MinZoom = 2;
            mapaPrincipal.MaxZoom = 19;
            mapaPrincipal.Zoom = 11;
            mapaPrincipal.Position = posicionDefectoMexico;
            mapaPrincipal.ShowCenter = false;
            mapaPrincipal.MouseWheelZoomEnabled = true;
            mapaPrincipal.CanDragMap = true;
            mapaPrincipal.DragButton = MouseButton.Left;

            AgregarMarcador(23.6345, -102.5528);

            mapaPrincipal.OnMapZoomChanged += MapaPrincipal_OnMapZoomChanged;

            ActualizarVisibilidadMarcadores();
        }

        private void AgregarMarcador(double lat, double lon)
        {
            var ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Fill = Brushes.Orange
            };

            var frameActual = Window.GetWindow(this).FindName("MarcoPrincipal") as Frame;

            var contenidoPopup = new MascotaToolTip();

            var popup = new Popup
            {
                PlacementTarget = ellipse,
                Placement = PlacementMode.Mouse,
                AllowsTransparency = true,
                StaysOpen = false,
                PopupAnimation = PopupAnimation.Fade,
                Child = contenidoPopup,
            };

            ellipse.MouseEnter += (s, e) =>
            {
                popup.IsOpen = true;
            };

            contenidoPopup.EventoDetallesMascota += (s, e) =>
            {
                NavegadorPrincipal.Instancia.Navegar(new ConsultarAdopcionExterna());
                popup.IsOpen = false;
            };

            var marcador = new GMapMarker(new PointLatLng(lat, lon))
            {
                Shape = ellipse,
                Offset = new Point(-15, -15)
            };

            mapaPrincipal.Markers.Add(marcador);
            marcadores.Add(marcador);
        }

        private void MapaPrincipal_OnMapZoomChanged()
        {
            ActualizarVisibilidadMarcadores();
        }

        private void ActualizarVisibilidadMarcadores()
        {
            bool visible = mapaPrincipal.Zoom >= ZoomVisible;

            foreach (var marcador in marcadores)
            {
                if (marcador.Shape != null)
                {
                    marcador.Shape.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }
    }
}
