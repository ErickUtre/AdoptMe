using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Lógica de interacción para RegistrarAdopcion.xaml
    /// </summary>
    public partial class RegistrarAdopcion : Page
    {
        private string rutaVideoSeleccionado;
        private Ubicacion _ubicacionSeleccionada = null;

        public RegistrarAdopcion()
        {
            InitializeComponent();
        }

        private void Btn_SubirFoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Selecciona una imagen"
            };

            // Mostrar el diálogo y verificar si se seleccionó un archivo
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Cargar la imagen en el control Image
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    Foto.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
        }

        private bool Validar_Campos()
        {
            if (string.IsNullOrWhiteSpace(tbNombreMascota.Text) ||
                string.IsNullOrWhiteSpace(tbEspecie.Text) ||
                string.IsNullOrWhiteSpace(tbRaza.Text) ||
                string.IsNullOrWhiteSpace(tbAño.Text) ||
                string.IsNullOrWhiteSpace(tbMes.Text) ||
                string.IsNullOrWhiteSpace(tbSexo.Text) ||
                string.IsNullOrWhiteSpace(tbTamaño.Text) ||
                string.IsNullOrWhiteSpace(tbDescripcion.Text) ||
                Foto.Source == null)

            {
                MessageBox.Show("Por favor, completa todos los campos.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validar que año y mes sean números válidos
            if (!int.TryParse(tbAño.Text, out int anio) || anio < 0 || anio > DateTime.Now.Year)
            {
                tbAño.Text = null;
                Utilidades.TextBoxExtensiones.SetTextoSugerido(tbAño, "Año invalido");

                if (!int.TryParse(tbMes.Text, out int mes) || mes < 1 || mes > 12)
                {
                    tbMes.Text = null;
                    Utilidades.TextBoxExtensiones.SetTextoSugerido(tbMes, "Mes invalido");

                    return false;
                }

                return false;
            }

            return true;
        }

        private void Btn_Registrar(object sender, RoutedEventArgs e)
        {
            if (Validar_Campos())
            {
                return;
            }
            else
            {
                Debug.WriteLine("HOLA");
                RegistroAdopcion();
            }
        }

        private void Btn_SubirVideo(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Title = "Selecciona un video";
            dialogo.Filter = "Archivos de video|*.mp4;*.avi;*.mov;*.wmv;*.mkv|Todos los archivos|*.*";
            dialogo.Multiselect = false;

            if (dialogo.ShowDialog() == true)
            {
                rutaVideoSeleccionado = dialogo.FileName;
                MessageBox.Show("Video seleccionado: " + rutaVideoSeleccionado);
                lb_RutaVideo.Content = "Ruta: " + rutaVideoSeleccionado;
            }
        }

        private void ObtenerUbicacion_Click(object sender, RoutedEventArgs e)
        {
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
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            bool? resultado = mapaRegistro.ShowDialog();

            if (resultado == true)
            {
                _ubicacionSeleccionada = mapaRegistro.ResultadoUbicacion;
            }
        }

        private async void RegistroAdopcion()
        {
            try
            {
                AdopcionServicios adopcionServicios = new AdopcionServicios();

                Ubicacion ubicacion = null;
                Debug.WriteLine("HOLA");
                Mascota mascota = new Mascota
                {
                    Nombre = tbNombreMascota.Text,
                    Especie = tbEspecie.Text,
                    Raza = tbRaza.Text,
                    Edad = tbAño.Text + "año(s) con" + tbMes.Text + " mes(es)",
                    Sexo = tbSexo.Text,
                    Tamaño = tbTamaño.Text,
                    Descripcion = tbDescripcion.Text
                };

                Debug.WriteLine("HOLA");
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

                Debug.WriteLine("HOLA");
                Adopcion adopcion = new Adopcion
                {
                    Estado = false,
                    PublicadorID = UsuarioSingleton.Instancia.UsuarioActual.UsuarioId,
                    Ubicacion = ubicacion,
                    Mascota = mascota
                };

                Debug.WriteLine("HOLA");
                HttpResponseMessage respuesta = await adopcionServicios.RegistrarAdopcionAsync(adopcion);
                Debug.WriteLine("HOLA");
                switch (respuesta.StatusCode)
                {
                    case HttpStatusCode.Created:
                        Debug.WriteLine("HOLA");
                        MessageBox.Show("Registro exitoso", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    default:
                        string detalles = await respuesta.Content.ReadAsStringAsync();
                        MessageBox.Show(
                            "Hubo un error al registrar la adopción. Por favor, intenta más tarde.",
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
                    $"Ocurrió un error inesperado con la adopción. Por favor, intenta más tarde.",
                    "Error con el servidor",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
