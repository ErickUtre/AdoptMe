using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cliente_AdoptMe.Vista
{
    public partial class RegistrarAdopcion : Page
    {
        private string rutaVideoSeleccionado;
        private Ubicacion _ubicacionSeleccionada = null;

        public RegistrarAdopcion()
        {
            InitializeComponent();
            CargarCombos();
        }

        private void CargarCombos()
        {
            for (int i = 0; i <= 40; i++)
                cbAño.Items.Add(i);

            for (int i = 0; i <= 12; i++)
                cbMes.Items.Add(i);

            cbSexo.Items.Add("Macho");
            cbSexo.Items.Add("Hembra");
        }

        private void Btn_SubirFoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Selecciona una imagen"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
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
                    return;
            }

            MapaRegistro mapaRegistro = new MapaRegistro()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            bool? resultado = mapaRegistro.ShowDialog();

            if (resultado == true)
                _ubicacionSeleccionada = mapaRegistro.ResultadoUbicacion;
        }

        private bool Validar_Campos()
        {
            if (string.IsNullOrWhiteSpace(tbNombreMascota.Text) ||
                string.IsNullOrWhiteSpace(tbEspecie.Text) ||
                string.IsNullOrWhiteSpace(tbRaza.Text) ||
                cbAño.SelectedItem == null ||
                cbMes.SelectedItem == null ||
                cbSexo.SelectedItem == null ||
                string.IsNullOrWhiteSpace(tbTamaño.Text) ||
                string.IsNullOrWhiteSpace(tbDescripcion.Text) ||
                Foto.Source == null ||
                _ubicacionSeleccionada == null)
            {
                MessageBox.Show("Por favor, completa todos los campos obligatorios.", "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(tbTamaño.Text, out int tamaño) || tamaño < 0 || tamaño > 300)
            {
                MessageBox.Show("El tamaño debe ser un número entre 0 y 300.", "Tamaño inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Btn_Registrar(object sender, RoutedEventArgs e)
        {
            if (!Validar_Campos())
                return;

            RegistroAdopcion();
        }

        private async void RegistroAdopcion()
        {
            try
            {
                AdopcionServicios adopcionServicios = new AdopcionServicios();

                Mascota mascota = new Mascota
                {
                    Nombre = tbNombreMascota.Text,
                    Especie = tbEspecie.Text,
                    Raza = tbRaza.Text,
                    Edad = $"{cbAño.SelectedItem} año(s) con {cbMes.SelectedItem} mes(es)",
                    Sexo = cbSexo.SelectedItem.ToString(),
                    Tamaño = $"{tbTamaño.Text} cm",
                    Descripcion = tbDescripcion.Text
                };

                Ubicacion ubicacion = new Ubicacion
                {
                    Longitud = _ubicacionSeleccionada.Longitud,
                    Latitud = _ubicacionSeleccionada.Latitud,
                    Ciudad = _ubicacionSeleccionada.Ciudad,
                    Estado = _ubicacionSeleccionada.Estado,
                    Pais = _ubicacionSeleccionada.Pais
                };

                Adopcion adopcion = new Adopcion
                {
                    Estado = false,
                    PublicadorID = UsuarioSingleton.Instancia.UsuarioActual.UsuarioId,
                    Ubicacion = ubicacion,
                    Mascota = mascota
                };

                HttpResponseMessage respuesta = await adopcionServicios.RegistrarAdopcionAsync(adopcion);

                switch (respuesta.StatusCode)
                {
                    case HttpStatusCode.Created:
                        MessageBoxResult confirmacion = MessageBox.Show(
                            "Registro exitoso.",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        if (confirmacion == MessageBoxResult.OK)
                        {
                            // Navegar al menú principal
                            NavigationService?.Navigate(new MapaPrincipal());
                        }
                        break;

                    default:
                        string detalles = await respuesta.Content.ReadAsStringAsync();
                        MessageBox.Show("Hubo un error al registrar la adopción. Por favor, intenta más tarde.", "Error del servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                        Registro.Error($"Error con el servidor {detalles}");
                        break;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                MessageBox.Show("Ocurrió un error inesperado con la adopción. Por favor, intenta más tarde.", "Error con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TbTamaño_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void tbTamaño_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
