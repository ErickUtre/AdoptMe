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
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipalAdministrador.xaml
    /// </summary>
    public partial class MenuPrincipalAdministrador : Window
    {
        public MenuPrincipalAdministrador()
        {
            InitializeComponent();
            MarcoPrincipal.NavigationService.Content = new MapaPrincipal();
        }

        private void BtnCerrarMenuPrincipal(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
