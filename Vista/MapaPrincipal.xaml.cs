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
using Cliente_AdoptMe.Modelo;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Cliente_AdoptMe.Servicios;
using Newtonsoft.Json;
using Cliente_AdoptMe.Grpc;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MapaPrincipal.xaml
    /// </summary>
    public partial class MapaPrincipal : Page
    {
        private List<GMapMarker> _marcadores = new List<GMapMarker>();
        private const int ZOOM_VISIBLE = 12;

        public MapaPrincipal()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Ubicacion ubicacionUsuario = UsuarioSingleton.Instancia.UsuarioActual.Ubicacion;

            if (ubicacionUsuario != null)
            {
                double? latitud = ubicacionUsuario.Latitud;
                double? longitud = ubicacionUsuario.Longitud;

                if (latitud.HasValue && longitud.HasValue)
                {
                    PointLatLng coordenadasUbicacion = new PointLatLng(latitud.Value, longitud.Value);
                    AgregarMarcadorUbicacionUsuario(coordenadasUbicacion);
                    MostrarMapaPrincipal(coordenadasUbicacion);
                    await MostrarAdopcionesCercanas(latitud.Value, longitud.Value);
                }
                else
                {
                    MostrarMapaPrincipal();
                }
            }
            else
            {
                MostrarMapaPrincipal();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            mapaPrincipal.Manager.CancelTileCaching();
            mapaPrincipal.Dispose();
            GMaps.Instance.CancelTileCaching();
        }
        
        private void MostrarMapaPrincipal(PointLatLng? ubicacionUsuario = null)
        {
            mapaPrincipal.MapProvider = GMapProviders.OpenStreetMap;

            var zoomInicial = 6;
            var posicionDefectoMexico = new PointLatLng(23.6345, -102.5528);
            var posicionAMostrar = posicionDefectoMexico;
            
            if (ubicacionUsuario.HasValue)
            {
                posicionAMostrar = ubicacionUsuario.Value;
                zoomInicial = 17;
            }

            mapaPrincipal.MinZoom = 2;
            mapaPrincipal.MaxZoom = 19;
            mapaPrincipal.Zoom = zoomInicial;
            mapaPrincipal.Position = posicionAMostrar;
            mapaPrincipal.ShowCenter = false;
            mapaPrincipal.MouseWheelZoomEnabled = true;
            mapaPrincipal.CanDragMap = true;
            mapaPrincipal.DragButton = MouseButton.Right;

            mapaPrincipal.OnMapZoomChanged -= MapaPrincipal_OnMapZoomChanged;
            mapaPrincipal.OnMapZoomChanged += MapaPrincipal_OnMapZoomChanged;

            ActualizarVisibilidadMarcadores();
        }

        private void AgregarMarcadorUbicacionUsuario(PointLatLng ubicacionUsuario)
        {
            var imagen = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoUbicacion.png")),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var marcador = new GMapMarker(ubicacionUsuario)
            {
                Shape = imagen,
                Offset = new Point(-25, -25)
            };

            mapaPrincipal.Markers.Add(marcador);
            _marcadores.Add(marcador);
        }

        private void AgregarMarcador(PointLatLng ubicacion, UbicacionGrpc.Mascota mascota, int adopcionId)
        {
            var imagen = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoAdopcion.png")),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var frameActual = Window.GetWindow(this).FindName("MarcoPrincipal") as Frame;

            var contenidoPopup = new MascotaToolTip();

            contenidoPopup.InicializarDatosMascota(mascota);

            var popup = new Popup
            {
                PlacementTarget = imagen,
                Placement = PlacementMode.Mouse,
                AllowsTransparency = true,
                StaysOpen = false,
                PopupAnimation = PopupAnimation.Fade,
                Child = contenidoPopup,
            };

            imagen.MouseEnter += (s, e) =>
            {
                popup.IsOpen = true;
            };

            contenidoPopup.EventoDetallesMascota += (s, e) =>
            {
                NavegadorPrincipal.Instancia.Navegar(new ConsultarAdopcionExterna(mascota, adopcionId));
                popup.IsOpen = false;
            };

            var marcador = new GMapMarker(ubicacion)
            {
                Shape = imagen,
                Offset = new Point(-15, -15)
            };

            mapaPrincipal.Markers.Add(marcador);
            _marcadores.Add(marcador);
        }

        private void MapaPrincipal_OnMapZoomChanged()
        {
            ActualizarVisibilidadMarcadores();
        }

        private void ActualizarVisibilidadMarcadores()
        {
            bool visible = mapaPrincipal.Zoom >= ZOOM_VISIBLE;

            foreach (var marcador in _marcadores)
            {
                if (marcador.Shape != null)
                {
                    marcador.Shape.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private async Task MostrarAdopcionesCercanas(double latitud, double longitud)
        {
            ServicioUbicacionGrpc servicioUbicacionGrpc = new ServicioUbicacionGrpc();
            var resultados = await servicioUbicacionGrpc.ObtenerAdopcionesCercanasAsync(latitud, longitud);

            if (resultados.Count > 0)
            {
                foreach (var adopcion in resultados)
                {
                    PointLatLng ubicacion = new PointLatLng(adopcion.Latitud, adopcion.Longitud);
                    UbicacionGrpc.Mascota mascota = adopcion.Mascota;
                    AgregarMarcador(ubicacion, mascota, adopcion.AdopcionId);
                }
            }
        }
    }
}
