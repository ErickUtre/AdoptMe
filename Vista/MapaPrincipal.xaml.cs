using Cliente_AdoptMe.Grpc;
using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json;
using NotificacionGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MapaPrincipal.xaml
    /// </summary>
    public partial class MapaPrincipal : Page
    {
        private List<GMapMarker> _marcadores = new List<GMapMarker>();
        private const int ZOOM_VISIBLE = 12;
        private GMapMarker _marcadorUsuario;
        private DateTime _ultimoMovimiento = DateTime.MinValue;
        private readonly TimeSpan _esperaAntesDeActualizar = TimeSpan.FromMilliseconds(600);
        private bool _actualizando = false;
        private PointLatLng _ultimoCentro = new PointLatLng();
        private int? _adopcionId;

        public MapaPrincipal(int adopcionId) : this()
        {
            _adopcionId = adopcionId;
        }

        public MapaPrincipal()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Ubicacion ubicacionUsuario = UsuarioSingleton.Instancia.UsuarioActual.Ubicacion;

            if (!UsuarioSingleton.Instancia.MostrarUbicacionRegistrada && UsuarioSingleton.Instancia.UbicacionActual.HasValue)
            {
                PointLatLng ubicacionActual = UsuarioSingleton.Instancia.UbicacionActual.Value;
                AgregarMarcadorUbicacionUsuario(ubicacionActual);
                MostrarMapaPrincipal(ubicacionActual);
                await MostrarAdopcionesCercanas(ubicacionActual.Lat, ubicacionActual.Lng);
            }
            else if (ubicacionUsuario != null)
            {
                double? latitud = ubicacionUsuario.Latitud;
                double? longitud = ubicacionUsuario.Longitud;

                if (latitud.HasValue && longitud.HasValue)
                {
                    PointLatLng coordenadasUbicacion = new PointLatLng(latitud.Value, longitud.Value);
                    AgregarMarcadorUbicacionUsuario(coordenadasUbicacion);

                    if (_adopcionId.HasValue)
                    {
                        MostrarAdopcionEspecifica();
                    }
                    else
                    {
                        MostrarMapaPrincipal(coordenadasUbicacion);
                    }
                        
                    await MostrarAdopcionesCercanas(latitud.Value, longitud.Value);
                }
                else
                {
                    MostrarMapaPrincipal();
                }
            }
            else
            {
                PointLatLng ubicacionDefecto = new PointLatLng(19.4326, -99.1332);
                AgregarMarcadorUbicacionUsuario(ubicacionDefecto);
                MostrarMapaPrincipal(ubicacionDefecto);
                await MostrarAdopcionesCercanas(ubicacionDefecto.Lat, ubicacionDefecto.Lng);
            }

            mapaPrincipal.OnMapDrag += async () =>
            {
                var ahora = DateTime.Now;

                if (_actualizando || ahora - _ultimoMovimiento < _esperaAntesDeActualizar)
                    return;

                _actualizando = true;
                _ultimoMovimiento = ahora;

                await Task.Delay(_esperaAntesDeActualizar);

                var centro = mapaPrincipal.Position;

                double deltaLat = Math.Abs(centro.Lat - _ultimoCentro.Lat);
                double deltaLng = Math.Abs(centro.Lng - _ultimoCentro.Lng);

                if (deltaLat >= 0.01 || deltaLng >= 0.01)
                {
                    _ultimoCentro = centro;
                    await ActualizarAdopcionesCercanasSegunCentro(centro.Lat, centro.Lng);
                }

                _actualizando = false;
            };
        }

        private async void MostrarAdopcionEspecifica()
        {
            AdopcionServicios adopcionServicios = new AdopcionServicios();
            Adopcion adopcion = await adopcionServicios.ObtenerAdopcionPorIdAsync(_adopcionId.Value);

            if (adopcion != null && adopcion.Ubicacion != null && adopcion.Mascota != null)
            {
                PointLatLng coordenadasAdopcionEspecifica = new PointLatLng(adopcion.Ubicacion.Latitud.Value, adopcion.Ubicacion.Longitud.Value);
                MostrarMapaPrincipal(coordenadasAdopcionEspecifica);

                UbicacionGrpc.Mascota mascota = new UbicacionGrpc.Mascota()
                {
                    MascotaId = adopcion.MascotaID,
                    Nombre = adopcion.Mascota.Nombre,
                    Edad = adopcion.Mascota.Edad,
                    Especie = adopcion.Mascota.Especie,
                    Raza = adopcion.Mascota.Raza,
                    Sexo = adopcion.Mascota.Sexo,
                    Tamano = adopcion.Mascota.Tamaño,
                    Descripcion = adopcion.Mascota.Descripcion
                };

                AgregarMarcador(coordenadasAdopcionEspecifica, mascota, adopcion.AdopcionID);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            mapaPrincipal.Manager.CancelTileCaching();
            mapaPrincipal.Dispose();
            GMaps.Instance.CancelTileCaching();
        }

        private void MostrarMapaPrincipal(PointLatLng? UbicacionAMostrar = null)
        {
            mapaPrincipal.MapProvider = GMapProviders.OpenStreetMap;

            var zoomInicial = 6;
            var posicionDefectoMexico = new PointLatLng(23.6345, -102.5528);
            var posicionAMostrar = posicionDefectoMexico;

            if (UbicacionAMostrar.HasValue)
            {
                posicionAMostrar = UbicacionAMostrar.Value;
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

            if (_marcadorUsuario != null)
            {
                mapaPrincipal.Markers.Remove(_marcadorUsuario);
                _marcadores.Remove(_marcadorUsuario);
                _marcadorUsuario = null;
            }

            _marcadorUsuario = new GMapMarker(ubicacionUsuario)
            {
                Shape = imagen,
                Offset = new Point(-25, -25)
            };

            mapaPrincipal.Markers.Add(_marcadorUsuario);
            _marcadores.Add(_marcadorUsuario);
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

        private async Task ActualizarAdopcionesCercanasSegunCentro(double latitud, double longitud)
        {
            foreach (var marcador in _marcadores.ToList())
            {
                if (marcador != _marcadorUsuario)
                {
                    mapaPrincipal.Markers.Remove(marcador);
                    _marcadores.Remove(marcador);
                }
            }

            ServicioUbicacionGrpc servicioUbicacionGrpc = new ServicioUbicacionGrpc();
            var resultados = await servicioUbicacionGrpc.ObtenerAdopcionesCercanasAsync(latitud, longitud);

            foreach (var adopcion in resultados)
            {
                PointLatLng ubicacion = new PointLatLng(adopcion.Latitud, adopcion.Longitud);
                AgregarMarcador(ubicacion, adopcion.Mascota, adopcion.AdopcionId);
            }

            ActualizarVisibilidadMarcadores();
        }

        private async void Btn_ToggleUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (!UsuarioSingleton.Instancia.MostrarUbicacionRegistrada)
            {
                var ubicacionUsuario = UsuarioSingleton.Instancia.UsuarioActual.Ubicacion;
                if (ubicacionUsuario != null && ubicacionUsuario.Latitud.HasValue && ubicacionUsuario.Longitud.HasValue)
                {
                    var coordenadas = new PointLatLng(ubicacionUsuario.Latitud.Value, ubicacionUsuario.Longitud.Value);
                    UsuarioSingleton.Instancia.UbicacionActual = coordenadas;
                    MostrarMapaPrincipal(coordenadas);
                    AgregarMarcadorUbicacionUsuario(coordenadas);
                    await MostrarAdopcionesCercanas(coordenadas.Lat, coordenadas.Lng);
                }

                UsuarioSingleton.Instancia.MostrarUbicacionRegistrada = true;
                Btn_ToggleUbicacion.Content = "Mostrar ubicación actual";
            }
            else
            {
                UbicacionServicios ubicacionServicios = new UbicacionServicios();
                var ubicacion = await ubicacionServicios.ObtenerUbicacionPorIPAsync();

                if (ubicacion != null && ubicacion.Latitud.HasValue && ubicacion.Longitud.HasValue)
                {
                    var coordenadas = new PointLatLng(ubicacion.Latitud.Value, ubicacion.Longitud.Value);
                    UsuarioSingleton.Instancia.UbicacionActual = coordenadas;
                    MostrarMapaPrincipal(coordenadas);
                    AgregarMarcadorUbicacionUsuario(coordenadas);
                    await MostrarAdopcionesCercanas(coordenadas.Lat, coordenadas.Lng);
                }

                UsuarioSingleton.Instancia.MostrarUbicacionRegistrada = false;
                Btn_ToggleUbicacion.Content = "Mostrar ubicación registrada";
            }
        }

    }
}