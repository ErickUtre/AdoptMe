using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
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
    /// Lógica de interacción para Reportes.xaml
    /// </summary>
    public partial class Reportes : Page
    {
        public Reportes()
        {
            InitializeComponent();
        }

        private void Btn_ReporteMascotasAdoptadas(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new Reporte());
        }

        private void Btn_ReporteMascotasAdopcion(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new Reporte());
        }
    }
}
