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
    public partial class MenuPrincipalUsuario : Window
    {
        public MenuPrincipalUsuario()
        {
            InitializeComponent();
            NavegadorPrincipal.Instancia.SetMarco(MarcoPrincipal);
            NavegadorPrincipal.Instancia.Navegar(new MapaPrincipal());
        }


        private void BtnCerrarMenuPrincipal(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnIrMapaPrincipal(object sender, RoutedEventArgs e)
        {
            var paginaActual = NavegadorPrincipal.Instancia.GetContenido();

            if (paginaActual == null || paginaActual.GetType() != typeof(MapaPrincipal))
            {
                NavegadorPrincipal.Instancia.Navegar(new MapaPrincipal());
            }
        }

        private void BtnIrRegistrarAdopcion(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new RegistrarAdopcion());
        }

        private void BtnIrVerAdopciones(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new ConsultarAdopciones());
        }

        private void BtnIrMensajes(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_IconoUsuario(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new ConsultarUsuario());
        }
    }
}
