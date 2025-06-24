using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// Lógica de interacción para InicioDeSesion.xaml
    /// </summary>
    public partial class InicioDeSesion : Window
    {
        private string _contraseña;
        public InicioDeSesion()
        {
            InitializeComponent();
        }

        private void PbCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void BtnIrVentanaRegistro_Click(object sender, RoutedEventArgs e)
        {
            RegistroDeUsuario ventana = new RegistroDeUsuario();
            this.Hide();
            ventana.ShowDialog();
            this.Show();
        }

        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            ReiniciarBordesCampos();
            if (ValidarDatosDeCampos())
            {
                IniciarSesionAsync();
            }
        }

        private async void IniciarSesionAsync()
        {
            AccesoServicios accesoServicios = new AccesoServicios();
            string contraseñaHash = Encriptador.GenerarHashSHA512(_contraseña);

            MostrarOverlay();
            ResultadoHttp resultadoHttp = await accesoServicios.IniciarSesionAsync(tbCorreo.Text, contraseñaHash);
            OcultarOverlay();

            if (!resultadoHttp.Exito)
            {
                if (resultadoHttp.Codigo == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show(
                        Properties.Resources.mensaje_CredencialesIncorrectas,
                        Properties.Resources.titulo_CredencialesIncorrectas,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                    
            }

            string cuerpoRespuesta = await resultadoHttp.Respuesta.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<RespuestaLogin>(cuerpoRespuesta);
            UsuarioSingleton.Instancia.IniciarSesion(resultado.Usuario, resultado.Token);

            if (!resultado.EsAdmin)
            {
                new MenuPrincipalUsuario().Show();
            }
            else
            {
                new MenuPrincipalAdministrador().Show();
            }

            this.Close();
        }

        private void ReiniciarBordesCampos()
        {
            tbCorreo.BorderBrush = Brushes.Transparent;
            tbContraseña.BorderBrush = Brushes.Transparent;
            pbContraseña.BorderBrush = Brushes.Transparent;
        }

        private bool ValidarDatosDeCampos()
        {
            bool correoValidado = !string.IsNullOrEmpty(tbCorreo.Text);
            bool contraseñaValidada;

            if (pbContraseña.Visibility == Visibility.Visible)
            {
                contraseñaValidada = !string.IsNullOrEmpty(pbContraseña.Password);
                _contraseña = pbContraseña.Password;
            }
            else
            {
                contraseñaValidada = !string.IsNullOrEmpty(tbContraseña.Text);
                _contraseña = tbContraseña.Text;
            }

            if (!correoValidado)
            {
                tbCorreo.BorderBrush = Brushes.Red;
            }

            if (!contraseñaValidada)
            {
                pbContraseña.BorderBrush = Brushes.Red;
                tbContraseña.BorderBrush = Brushes.Red;
            }

            return correoValidado && contraseñaValidada;
        }

        private void CambiarVisibilidadContraseña_Click(object sender, RoutedEventArgs e)
        {
            if (tbtn_VisibilidadContraseña.IsChecked == true)
            {
                tbContraseña.Text = pbContraseña.Password;
                pbContraseña.Visibility = Visibility.Collapsed;
                tbContraseña.Visibility = Visibility.Visible;

                img_VisibilidadContraseña.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoMostrar.png"));
            }
            else
            {
                pbContraseña.Password = tbContraseña.Text;
                pbContraseña.Visibility = Visibility.Visible;
                tbContraseña.Visibility = Visibility.Collapsed;

                img_VisibilidadContraseña.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoOcultar.png"));
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
