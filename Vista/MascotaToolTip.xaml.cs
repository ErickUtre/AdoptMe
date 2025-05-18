using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MascotaToolTip.xaml
    /// </summary>
    public partial class MascotaToolTip : UserControl
    {
        public event EventHandler EventoDetallesMascota;

        public MascotaToolTip()
        {
            InitializeComponent();
        }

        private void IrDetallesMascota(object sender, RoutedEventArgs e)
        {
            EventoDetallesMascota?.Invoke(this, EventArgs.Empty);
        }
    }
}
