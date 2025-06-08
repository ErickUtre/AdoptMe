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
                IniciarSesion();
            }
        }

        private async void IniciarSesion()
        {
            try
            {
                AccesoServicios accesoServicios = new AccesoServicios();
                string contraseñaHash = Encriptador.GenerarHashSHA512(_contraseña);

                HttpResponseMessage respuestaHttp = await accesoServicios.IniciarSesionAsync(tbCorreo.Text, contraseñaHash);
                string cuerpoRespuesta = await respuestaHttp.Content.ReadAsStringAsync();

                switch (respuestaHttp.StatusCode)
                {
                    case HttpStatusCode.OK:       
                        var resultado = JsonConvert.DeserializeObject<RespuestaLogin>(cuerpoRespuesta);
                        UsuarioSingleton.Instancia.IniciarSesion(resultado.Usuario, resultado.Token);

                        if (!resultado.EsAdmin)
                        {
                            MenuPrincipalUsuario menuPrincipalUsuario = new MenuPrincipalUsuario();
                            menuPrincipalUsuario.Show();
                            this.Close();
                        } 
                        else
                        {
                            MenuPrincipalAdministrador menuPrincipalAdministrador = new MenuPrincipalAdministrador();
                            menuPrincipalAdministrador.Show();
                            this.Close();
                        }
                        
                        break;

                    case HttpStatusCode.Unauthorized:
                        MessageBox.Show(
                            "El correo y/o contraseña son incorrectos.",
                            "Credenciales incorrectas",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;

                    default:
                        MessageBox.Show(
                            "Hubo un error al iniciar sesión. Por favor, intenta más tarde.",
                            "Error del servidor",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        Registro.Error($"Error con el servidor {cuerpoRespuesta}");
                        break;

                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                MessageBox.Show(
                    $"Ocurrió un error inesperado con el servidor. Por favor, intenta más tarde.",
                    "Error con el servidor",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
    }
}
