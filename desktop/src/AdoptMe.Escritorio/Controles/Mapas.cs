using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AdoptMe.Escritorio.Nucleo.Modelos;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace AdoptMe.Escritorio.Controles;

public static class Mapas
{
    public const string IconoUbicacion = "pack://application:,,,/Recursos/Imagenes/IconoUbicacion.png";
    public const string IconoAdopcion = "pack://application:,,,/Recursos/Imagenes/IconoAdopcion.png";

    public static void Configurar(GMapControl mapa)
    {
        ArgumentNullException.ThrowIfNull(mapa);
        mapa.MapProvider = GMapProviders.OpenStreetMap;
        mapa.MinZoom = 2;
        mapa.MaxZoom = 19;
        mapa.ShowCenter = false;
        mapa.MouseWheelZoomEnabled = true;
        mapa.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
        mapa.CanDragMap = true;
        mapa.DragButton = MouseButton.Left;
    }

    public static PointLatLng APunto(Coordenadas coordenadas)
    {
        ArgumentNullException.ThrowIfNull(coordenadas);
        return new PointLatLng(coordenadas.Latitud, coordenadas.Longitud);
    }

    public static Coordenadas ACoordenadas(PointLatLng punto) => new(punto.Lat, punto.Lng);

    public static GMapMarker CrearMarcador(Coordenadas coordenadas, string icono, double tamano, object? etiqueta = null) =>
        new(APunto(coordenadas))
        {
            Shape = new Image
            {
                Width = tamano,
                Height = tamano,
                Source = new BitmapImage(new Uri(icono)),
                Cursor = Cursors.Hand
            },
            Offset = new Point(-tamano / 2, -tamano),
            Tag = etiqueta
        };
}
