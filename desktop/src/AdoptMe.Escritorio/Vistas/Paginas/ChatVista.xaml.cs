using System.Windows.Controls;

namespace AdoptMe.Escritorio.Vistas.Paginas;

public partial class ChatVista : UserControl
{
    public ChatVista()
    {
        InitializeComponent();
    }

    private void AlDesplazar(object remitente, ScrollChangedEventArgs argumentos)
    {
        if (argumentos.ExtentHeightChange > 0 && remitente is ScrollViewer desplazamiento)
        {
            desplazamiento.ScrollToEnd();
        }
    }
}
