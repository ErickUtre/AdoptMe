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

        public void InicializarDatosMascota(UbicacionGrpc.Mascota mascota)
        {
            txtbl_Nombre.Text = mascota.Nombre;
            txtbl_Edad.Text += $": {mascota.Edad}";
            txtbl_Sexo.Text += $": {mascota.Sexo}";
            txtbl_Especie.Text += $": {mascota.Especie}";
            txtbl_Raza.Text += $": {mascota.Raza}";
        }

        private void IrDetallesMascota(object sender, RoutedEventArgs e)
        {
            EventoDetallesMascota?.Invoke(this, EventArgs.Empty);
        }
    }
}
