using Cliente_AdoptMe.Modelo;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarAdopcion.xaml
    /// </summary>
    public partial class ConsultarAdopcion : Page
    {
        private Adopcion _adopcion;

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
                    // Imagen por defecto si no hay foto
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

        private void Btn_EditarNombre(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Nombre);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Nombre.Text = "Nombre: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Nombre = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarEspecie(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Especie);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Especie.Text = "Especie: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Especie = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarRaza(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Raza);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Raza.Text = "Raza: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Raza = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarEdad(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Edad);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Edad.Text = "Edad: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Edad = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarSexo(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Sexo);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Sexo.Text = "Sexo: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Sexo = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarTamaño(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Tamaño);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Tamaño.Text = "Tamaño: " + editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Tamaño = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarDescripcion(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(_adopcion.Mascota?.Descripcion);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Descripcion.Text = editarCampo.NuevoValor;
                if (_adopcion.Mascota != null)
                    _adopcion.Mascota.Descripcion = editarCampo.NuevoValor;

                // TODO: Guardar cambio en base de datos
            }
        }

        private void Btn_EditarFoto(object sender, RoutedEventArgs e)
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

                    FotoMascota.Source = bitmap;
                    FotoComplemento.Source = bitmap;

                    if (_adopcion != null)
                        _adopcion.Foto = bitmap;

                    // TODO: Guardar la imagen en base de datos o servidor
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
        }

        private void Btn_VerVideo(object sender, RoutedEventArgs e)
        {
            // Aquí pondrías la lógica para reproducir video, si tienes video asociado
            MessageBox.Show("Funcionalidad de video aún no implementada.");
        }
    }
}
