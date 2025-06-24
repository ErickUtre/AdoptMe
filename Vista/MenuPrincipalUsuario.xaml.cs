using Cliente_AdoptMe.Servicios;
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
            InicializarDatos();
        }

        private async void InicializarDatos()
        {
            txtblNombreUsuario.Text = UsuarioSingleton.Instancia.UsuarioActual.Nombre;
            await MostrarFotoAsync();
        }

        private void Btn_CerrarMenuPrincipal(object sender, RoutedEventArgs e)
        {
            UsuarioSingleton.Instancia.CerrarSesion();
            InicioDeSesion inicioDeSesion = new InicioDeSesion();
            inicioDeSesion.Show();
            this.Close();
        }

        private void Btn_IrMapaPrincipal(object sender, RoutedEventArgs e)
        {
            var paginaActual = NavegadorPrincipal.Instancia.GetContenido();

            if (paginaActual == null || paginaActual.GetType() != typeof(MapaPrincipal))
            {
                NavegadorPrincipal.Instancia.Navegar(new MapaPrincipal());
            }
        }

        private void Btn_IrRegistrarAdopcion(object sender, RoutedEventArgs e)
        {
            var paginaActual = NavegadorPrincipal.Instancia.GetContenido();

            if (paginaActual == null || paginaActual.GetType() != typeof(RegistrarAdopcion))
            {
                NavegadorPrincipal.Instancia.Navegar(new RegistrarAdopcion());
            }
        }

        private void Btn_IrVerAdopciones(object sender, RoutedEventArgs e)
        {
            var paginaActual = NavegadorPrincipal.Instancia.GetContenido();

            if (paginaActual == null || paginaActual.GetType() != typeof(ConsultarAdopciones))
            {
                NavegadorPrincipal.Instancia.Navegar(new ConsultarAdopciones());
            }
        }

        private void Btn_IconoUsuario(object sender, RoutedEventArgs e)
        {
            ConsultarUsuario consultarUsuario = new ConsultarUsuario();

            consultarUsuario.EventoActualizarFotoPerfil += (s, args) =>
            {
                FotoPerfil.Source = UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil;
            };

            consultarUsuario.EventoActualizarNombre += (s, args) =>
            {
                Console.WriteLine(UsuarioSingleton.Instancia.UsuarioActual.Nombre);
                txtblNombreUsuario.Text = UsuarioSingleton.Instancia.UsuarioActual.Nombre;
            };

            var paginaActual = NavegadorPrincipal.Instancia.GetContenido();

            if (paginaActual == null || paginaActual.GetType() != typeof(ConsultarUsuario))
            {
                NavegadorPrincipal.Instancia.Navegar(consultarUsuario);
            }
        }

        private async Task MostrarFotoAsync()
        {
            var imagen = await InterfazUsuarioHelper.ObtenerFotoPerfilAsync(UsuarioSingleton.Instancia.Token);
            if (imagen != null)
            {
                FotoPerfil.Source = imagen;
                UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil = imagen;
            }
        }

        public void MostrarOverlay()
        {
            CargandoOverlay.Visibility = Visibility.Visible;
            CargandoOverlay.IsHitTestVisible = true;
        }

        public void OcultarOverlay()
        {
            CargandoOverlay.Visibility = Visibility.Collapsed;
            CargandoOverlay.IsHitTestVisible = false;
        }

    }
}
