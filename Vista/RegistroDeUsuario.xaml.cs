using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    /// Lógica de interacción para RegistroDeUsuario.xaml
    /// </summary>
    public partial class RegistroDeUsuario : Window
    {
        private Ubicacion _ubicacionSeleccionada = null;
        private string _contraseña;
        private bool hayPermisoDeObtenerUbicacionUsuario = false;

        public RegistroDeUsuario()
        {
            InitializeComponent();
        }

        private void PbCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void BtnIrVentanaInicioDeSesion_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ObtenerUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (!hayPermisoDeObtenerUbicacionUsuario)
            {
                MessageBoxResult respuesta = MessageBox.Show(
                    Properties.Resources.mensaje_PermitirUbicacion,
                    Properties.Resources.titulo_PermisosUbicacion,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if ( respuesta == MessageBoxResult.Yes )
                {
                    hayPermisoDeObtenerUbicacionUsuario = true;
                }
                else
                {
                    return;
                }
            }

            if (_ubicacionSeleccionada != null)
            {
                MessageBoxResult respuesta = MessageBox.Show(
                    Properties.Resources.txtbl_SeguroDeModificarUbicacion,
                    Properties.Resources.titulo_ConfirmarModificacion,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (respuesta == MessageBoxResult.No)
                {
                    return;
                }
            }

            MapaRegistro mapaRegistro = new MapaRegistro()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            bool? resultado = mapaRegistro.ShowDialog();

            if (resultado == true)
            {
                _ubicacionSeleccionada = mapaRegistro.ResultadoUbicacion;
            }
        }

        private void Registrar_Click(object sender, RoutedEventArgs e)
        {
            ReiniciarBordesCampos();
            if (ValidarDatosDeCampos())
            {
                RegistrarUsuario();
            }
        }

        private async void RegistrarUsuario()
        {
            try
            {
                UsuarioServicios usuarioServicios = new UsuarioServicios();

                string contraseñaHash = Encriptador.GenerarHashSHA512(_contraseña);
                Ubicacion ubicacion = null;

                Acceso acceso = new Acceso
                {
                    Correo = tbCorreo.Text,
                    ContrasenaHash = contraseñaHash,
                    EsAdmin = false
                };

                if (_ubicacionSeleccionada != null)
                {
                    ubicacion = new Ubicacion
                    {
                        Longitud = _ubicacionSeleccionada.Longitud,
                        Latitud = _ubicacionSeleccionada.Latitud,
                        Ciudad = _ubicacionSeleccionada.Ciudad,
                        Estado = _ubicacionSeleccionada.Estado,
                        Pais = _ubicacionSeleccionada.Pais
                    };
                }

                Usuario usuario = new Usuario
                {
                    Nombre = tbNombre.Text,
                    Telefono = tbTelefono.Text,
                    Acceso = acceso,
                    Ubicacion = ubicacion
                };

                HttpResponseMessage respuesta = await usuarioServicios.RegistrarUsuarioAsync(usuario);

                switch (respuesta.StatusCode)
                {
                    case HttpStatusCode.Created:
                        MessageBox.Show(
                            "Registro exitoso", 
                            "Éxito", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Information);

                        this.Close();
                        break;

                    case HttpStatusCode.Conflict:
                        MessageBox.Show("El correo ya está registrado", 
                            "Advertencia", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Warning);
                        break;

                    default:
                        string detalles = await respuesta.Content.ReadAsStringAsync();
                        MessageBox.Show(
                            "Hubo un error al registrar al usuario. Por favor, intenta más tarde.",
                            "Error del servidor",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        Registro.Error($"Error con el servidor {detalles}");
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
            InterfazUsuarioHelper.ReiniciarBordesTextBox(new[] {
                tbNombre, tbCorreo, tbTelefono, tbContraseña, tbConfirmarContraseña
            }, Brushes.Transparent);

            InterfazUsuarioHelper.ReiniciarBordesPasswordBox(new[] {
                pbContraseña, pbConfirmarContraseña
            }, Brushes.Transparent);
        }

        private bool ValidarDatosDeCampos()
        {
            bool nombreValidado = Validador.ValidarNombre(tbNombre.Text);
            bool correoValidado = Validador.ValidarCorreo(tbCorreo.Text);
            bool telefonoValidado = Validador.ValidarTelefono(tbTelefono.Text);
            bool contraseñaValidada;
            bool confirmacionContraseñaValidada;

            if (pbContraseña.Visibility == Visibility.Visible)
            {
                contraseñaValidada = Validador.ValidarContraseña(pbContraseña.Password);
                _contraseña = pbContraseña.Password;
            }
            else
            {
                contraseñaValidada = Validador.ValidarContraseña(tbContraseña.Text);
                _contraseña = tbContraseña.Text;
            }

            if (pbConfirmarContraseña.Visibility == Visibility.Visible)
            {
                if (Validador.ValidarContraseña(pbConfirmarContraseña.Password))
                {
                    confirmacionContraseñaValidada = Validador.EsMismaContraseña(_contraseña, pbConfirmarContraseña.Password);
                }
                else
                {
                    confirmacionContraseñaValidada = false;
                }
            }
            else
            {
                if (Validador.ValidarContraseña(tbConfirmarContraseña.Text))
                {
                    confirmacionContraseñaValidada = Validador.EsMismaContraseña(_contraseña, tbConfirmarContraseña.Text);
                }
                else
                {
                    confirmacionContraseñaValidada = false;
                }
            }

            if (!nombreValidado)
            {
                tbNombre.BorderBrush = Brushes.Red;
            }

            if (!correoValidado)
            {
                tbCorreo.BorderBrush = Brushes.Red;
            }

            if (!telefonoValidado)
            {
                tbTelefono.BorderBrush = Brushes.Red;
            }

            if (!contraseñaValidada)
            {
                pbContraseña.BorderBrush = Brushes.Red;
                tbContraseña.BorderBrush = Brushes .Red;
            }

            if (!confirmacionContraseñaValidada)
            {
                pbConfirmarContraseña.BorderBrush = Brushes.Red;
                tbConfirmarContraseña .BorderBrush = Brushes .Red;
            }

            return nombreValidado && correoValidado && telefonoValidado
                && contraseñaValidada && confirmacionContraseñaValidada;
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

        private void CambiarVisibilidadConfirmarContraseña_Click(object sender, RoutedEventArgs e)
        {
            if (tbtn_VisibilidadConfirmarContraseña.IsChecked == true)
            {
                tbConfirmarContraseña.Text = pbConfirmarContraseña.Password;
                pbConfirmarContraseña.Visibility = Visibility.Collapsed;
                tbConfirmarContraseña.Visibility = Visibility.Visible;

                img_VisibilidadConfirmarContraseña.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoMostrar.png"));
            }
            else
            {
                pbConfirmarContraseña.Password = tbConfirmarContraseña.Text;
                pbConfirmarContraseña.Visibility = Visibility.Visible;
                tbConfirmarContraseña.Visibility = Visibility.Collapsed;

                img_VisibilidadConfirmarContraseña.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/IconoOcultar.png"));
            }
        }
    }
}
