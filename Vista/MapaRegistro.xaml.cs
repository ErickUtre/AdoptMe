using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MapaRegistro.xaml
    /// </summary>
    public partial class MapaRegistro : Window
    {
        private GMapMarker posicionActual = null;
        public Ubicacion ResultadoUbicacion { get; set; }

        public MapaRegistro()
        {
            InitializeComponent();
            _ = CargarMapaConUbicacionPorIPAsync();
            mapaPrincipal.MouseDoubleClick += MapaPrincipal_MouseDoubleClick;
        }

        private async Task CargarMapaConUbicacionPorIPAsync()
        {
            mapaPrincipal.MapProvider = GMapProviders.OpenStreetMap;

            UbicacionServicios ubicacionServicios = new UbicacionServicios();
            Ubicacion ubicacionPorIP = await ubicacionServicios.ObtenerUbicacionPorIPAsync();

            if (ubicacionPorIP.UbicacionID == -1)
            {
                MessageBox.Show(Properties.Resources.mensaje_ErrorUbicacion);
                return;
            }

            ResultadoUbicacion = ubicacionPorIP;
            double latitud = ubicacionPorIP.Latitud.Value;
            double longitud = ubicacionPorIP.Longitud.Value;
            MostrarDatosUbicacion(ubicacionPorIP);

            var posicion = new PointLatLng(latitud, longitud);

            mapaPrincipal.MinZoom = 2;
            mapaPrincipal.MaxZoom = 19;
            mapaPrincipal.Zoom = 15;
            mapaPrincipal.Position = posicion;
            mapaPrincipal.ShowCenter = false;
            mapaPrincipal.MouseWheelZoomEnabled = true;
            mapaPrincipal.CanDragMap = true;
            mapaPrincipal.DragButton = MouseButton.Right;

            AgregarMarcador(latitud, longitud);
        }

        private async void MapaPrincipal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var punto = e.GetPosition(mapaPrincipal);
            var posicion = mapaPrincipal.FromLocalToLatLng((int)punto.X, (int)punto.Y);

            UbicacionServicios ubicacionServicios = new UbicacionServicios();
            Ubicacion datosUbicacion = await ubicacionServicios.ObtenerDireccionPorCoordenadasAsync(posicion.Lat, posicion.Lng);

            if (datosUbicacion.UbicacionID != -1)
            {
                ResultadoUbicacion = datosUbicacion;
                AgregarMarcador(posicion.Lat, posicion.Lng);
                MostrarDatosUbicacion(datosUbicacion);
            }
            else
            {
                MessageBox.Show(Properties.Resources.mensaje_ErrorUbicacion);
            }
        }

        private void AgregarMarcador(double lat, double lon)
        {
            if (posicionActual != null)
            {
                mapaPrincipal.Markers.Remove(posicionActual);
            }

            var imagen = new Image
            {
                Width = 30,
                Height = 30,
                Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoUbicacion.png")),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            var marcador = new GMapMarker(new PointLatLng(lat, lon))
            {
                Shape = imagen,
                Offset = new Point(-15, -15)
            };

            posicionActual = marcador;
            mapaPrincipal.Markers.Add(marcador);
        }

        private void MostrarDatosUbicacion(Ubicacion ubicacion)
        {
            datosUbicacion.Text = ubicacion.ToString();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
