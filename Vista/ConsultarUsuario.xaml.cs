using Cliente_AdoptMe.Grpc.ServiciosGrpc;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using MultimediaGrpc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarUsuario.xaml
    /// </summary>
    public partial class ConsultarUsuario : Page
    {
        public ConsultarUsuario()
        {
            InitializeComponent();
            InicializarDatosUsuario();
        }

        private void BtnExpandirImagen(object sender, RoutedEventArgs e)
        {
            ImagenExpandida imagenExpandida = new ImagenExpandida(Foto);
            imagenExpandida.ShowDialog();
        }

        private void InicializarDatosUsuario()
        {
            //LOGICA PARA INICIALIZAR LOS DATOS DEL USUARIO
            FotoComplemento.Source = UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil;
        }

        private void Btn_EditarNombre(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo();
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Nombre.Text = "Nombre: " + editarCampo.nuevoValor;  
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarCorreo(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo();
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Correo.Text = "Correo: " + editarCampo.nuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarTelefono(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo();
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Telefono.Text = "Teléfono: " + editarCampo.nuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarCiudad(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo();
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Ciudad.Text = "Ciudad: " + editarCampo.nuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private async void Btn_EditarFoto(object sender, RoutedEventArgs e)
        {
            /*
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
                    FotoComplemento.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
            //SE GUARDA EN LA BASE DE DATOS
            */
            string rutaArchivo;
            
            var dlg = new OpenFileDialog();
            dlg.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (dlg.ShowDialog() == true)
            {
                rutaArchivo = dlg.FileName;

                if (!File.Exists(rutaArchivo))
                {
                    MessageBox.Show("El archivo no existe.");
                    return;
                }

                await SubirArchivoAsync(rutaArchivo);
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
            var imagen = await InterfazUsuarioHelper.ObtenerFotoPerfilAsync(UsuarioSingleton.Instancia.Token);
            if (imagen != null)
            {
                FotoComplemento.Source = imagen;
                UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil = imagen;
            }
            else
            {
                FotoComplemento.Source = null;
            }
        }

        private async Task SubirArchivoAsync(string rutaArchivo)
        {
            try
            {
                ServicioMultimediaGrpc servicioMultimedia = new ServicioMultimediaGrpc();
                await servicioMultimedia.SubirArchivoAsync(
                            rutaArchivo,
                            UsuarioSingleton.Instancia.UsuarioActual.UsuarioId,
                            UsuarioSingleton.Instancia.Token,
                            metadata => servicioMultimedia.Cliente.SubirFotoUsuario(metadata),
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

    }
}
