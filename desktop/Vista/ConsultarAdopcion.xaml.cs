using Cliente_AdoptMe.Grpc.ServiciosGrpc;
using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Cliente_AdoptMe.Utilidades.InterfazUsuarioHelper;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarAdopcion.xaml
    /// </summary>
    public partial class ConsultarAdopcion : Page
    {
        private Adopcion _adopcion;
        private readonly AdopcionServicios _adopcionServicios = new AdopcionServicios();

        public ConsultarAdopcion(Adopcion adopcion)
        {
            InitializeComponent();
            if (adopcion == null)
            {
                MessageBox.Show("No se recibió información de la adopción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _adopcion = adopcion;
            InicializarDatosAdopcion(_adopcion);
        }

        private void InicializarDatosAdopcion(Adopcion adopcion)
        {
            var mascota = adopcion.Mascota;

            Txt_Nombre.Text = "Nombre: " + (mascota?.Nombre ?? "N/D");
            Txt_Especie.Text = "Especie: " + (mascota?.Especie ?? "N/D");
            Txt_Raza.Text = "Raza: " + (mascota?.Raza ?? "N/D");
            Txt_Edad.Text = "Edad: " + (mascota?.Edad ?? "N/D");
            Txt_Sexo.Text = "Sexo: " + (mascota?.Sexo ?? "N/D");
            Txt_Tamaño.Text = "Tamaño: " + (mascota?.Tamaño ?? "N/D");
            Txt_Descripcion.Text = !string.IsNullOrWhiteSpace(mascota?.Descripcion) ? mascota.Descripcion : "Sin descripción";

            try
            {
                if (adopcion.Foto != null)
                {
                    FotoMascota.Source = adopcion.Foto;
                    FotoComplemento.Source = adopcion.Foto;
                }
                else
                {
                    var defaultUri = new Uri("pack://application:,,,/Recursos/Imagenes/MascotaDefault.png");
                    FotoMascota.Source = new BitmapImage(defaultUri);
                    FotoComplemento.Source = new BitmapImage(defaultUri);
                }
            }
            catch
            {
                var defaultUri = new Uri("pack://application:,,,/Recursos/Imagenes/MascotaDefault.png");
                FotoMascota.Source = new BitmapImage(defaultUri);
                FotoComplemento.Source = new BitmapImage(defaultUri);
            }
        }

        private void BtnExpandirImagen(object sender, RoutedEventArgs e)
        {
            ImagenExpandida imagenExpandida = new ImagenExpandida(FotoMascota);
            imagenExpandida.ShowDialog();
        }

        private async System.Threading.Tasks.Task GuardarCambiosAsync()
        {
            try
            {
                var response = await _adopcionServicios.ModificarAdopcionAsync(_adopcion.AdopcionID, _adopcion);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Error al actualizar adopción: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al actualizar adopción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Btn_EditarNombre(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("nombre", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Nombre.Text = "Nombre: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Nombre = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarEspecie(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("especie", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Especie.Text = "Especie: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Especie = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarRaza(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("raza", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Raza.Text = "Raza: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Raza = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarEdad(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("edad", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Edad.Text = "Edad: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Edad = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarSexo(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("sexo", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Sexo.Text = "Sexo: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Sexo = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarTamaño(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("tamaño", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Tamaño.Text = "Tamaño: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Tamaño = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarDescripcion(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo("descripcion", _adopcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Descripcion.Text = editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Descripcion = editarCampo.NuevoValor;

                await GuardarCambiosAsync();
            }
        }

        private async void Btn_EditarFoto(object sender, RoutedEventArgs e)
        {
            string rutaArchivo;

            var dialogo = new OpenFileDialog();
            dialogo.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dialogo.ShowDialog() == true)
            {
                rutaArchivo = dialogo.FileName;

                if (!File.Exists(rutaArchivo))
                {
                    MessageBox.Show("El archivo no existe.");
                    return;
                }

                await SubirArchivoAsync(rutaArchivo);
                await Task.Delay(1000);
                await MostrarFotoAsync();
            }
            else
            {
                MessageBox.Show("Seleccione un archivo primero.");
                return;
            }
        }

        private async Task MostrarFotoAsync()
        {
            var imagen = await ObtenerFotoMascotaAsync(_adopcion.MascotaID, UsuarioSingleton.Instancia.Token, true);
            if (imagen != null)
            {
                FotoComplemento.Source = imagen;
                FotoMascota.Source = imagen;
            }
        }

        private async Task SubirArchivoAsync(string rutaArchivo)
        {
            try
            {
                ServicioMultimediaGrpc servicioMultimedia = new ServicioMultimediaGrpc();
                await servicioMultimedia.SubirArchivoAsync(
                            rutaArchivo,
                            _adopcion.MascotaID,
                            UsuarioSingleton.Instancia.Token,
                            metadata => servicioMultimedia.Cliente.SubirFotoMascota(metadata),
                            new[] { ".jpg", ".jpeg", ".png" }
                        );

            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                MessageBox.Show(
                    Properties.Resources.mensaje_ErrorServidor,
                    Properties.Resources.global_ErrorServidor,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void Btn_VerVideo(object sender, RoutedEventArgs e)
        {
            await MostrarVideoMascotaAsync(_adopcion.MascotaID, UsuarioSingleton.Instancia.Token);
        }

        public async Task MostrarVideoMascotaAsync(int idMascota, string token)
        {
            var msVideo = await ObtenerVideoMascotaAsync(idMascota, token);
            if (msVideo != null)
            {
                string rutaTemporal = await GuardarVideoTemporalAsync(msVideo, idMascota);
                var ventanaVideo = new Video(rutaTemporal);
                ventanaVideo.ShowDialog();
            }
            else
            {
                MessageBox.Show("No se pudo descargar el video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task<string> GuardarVideoTemporalAsync(MemoryStream ms, int idMascota)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"video_mascota_{idMascota}.mp4");

            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                ms.Position = 0;
                await ms.CopyToAsync(fileStream);
            }

            return tempFile;
        }

    }
}
