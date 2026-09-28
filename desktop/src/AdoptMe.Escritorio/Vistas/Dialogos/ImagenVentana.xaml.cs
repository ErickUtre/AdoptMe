using System.Windows;
using AdoptMe.Escritorio.Conversores;

namespace AdoptMe.Escritorio.Vistas.Dialogos;

public partial class ImagenVentana : Window
{
    public ImagenVentana(byte[] imagen)
    {
        InitializeComponent();
        Imagen.Source = Imagenes.DesdeBytes(imagen);
    }
}
