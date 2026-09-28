using System.Windows;
using System.Windows.Controls;

namespace AdoptMe.Escritorio.Vistas.Paginas;

public partial class TarjetaMascota : UserControl
{
    public TarjetaMascota()
    {
        InitializeComponent();
    }

    public event EventHandler? DetallesSolicitados;

    private void AlSolicitarDetalles(object remitente, RoutedEventArgs argumentos) => DetallesSolicitados?.Invoke(this, EventArgs.Empty);
}
