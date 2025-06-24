using Cliente_AdoptMe.Modelo;
using Microsoft.Win32;
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
    /// <summary>
    /// Lógica de interacción para ConsultarAdopcion.xaml
    /// </summary>
    public partial class ConsultarAdopcion : Page
    {
        public ConsultarAdopcion(Mascota mascota)
        {
            InitializeComponent();
            InicializarDatosMascota(mascota);
        }

        private void InicializarDatosMascota(Mascota mascota)
        {
            Txt_Nombre.Text = "Nombre: " + mascota.Nombre;
            Txt_Especie.Text = "Especie: " + mascota.Especie;
            Txt_Raza.Text = "Raza: " + mascota.Raza;
            Txt_Edad.Text = "Edad: " + mascota.Edad;
            Txt_Sexo.Text = "Sexo: " + mascota.Sexo;
            Txt_Tamaño.Text = "Tamaño: " + mascota.Tamaño;
            Txt_Descripcion.Text = "Descripcion: " + mascota.Descripcion;
            FotoMascota.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/Dalmata.png"));
            FotoComplemento.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/Bongo.png"));
        }

        private void BtnExpandirImagen(object sender, RoutedEventArgs e)
        {
            ImagenExpandida imagenExpandida = new ImagenExpandida(FotoMascota);
            imagenExpandida.ShowDialog();
        }
        
        private void Btn_EditarNombre(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Nombre.Text = "Nombre: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarEspecie(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Especie.Text = "Especie: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarRaza(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Raza.Text = "Raza: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarEdad(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Edad.Text = "Edad: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarSexo(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Sexo.Text = "Sexo: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarTamaño(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Tamaño.Text = "Tamaño: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarDescripcion(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(null);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                Txt_Descripcion.Text = "Descrpcion: " + editarCampo.NuevoValor;
            }

            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_EditarFoto(object sender, RoutedEventArgs e)
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

                    FotoMascota.Source = bitmap;
                    FotoComplemento.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
            //SE GUARDA EN LA BASE DE DATOS
        }

        private void Btn_VerVideo(object sender, RoutedEventArgs e)
        {
            string rutaVideo = @"C:\Users\Erick\Downloads\videoplayback.mp4";
            Video reproductor = new Video(rutaVideo);
            reproductor.ShowDialog();

            /*if (!string.IsNullOrEmpty(rutaVideoSeleccionado))
            {
                VentanaReproductor reproductor = new VentanaReproductor(rutaVideoSeleccionado);
                reproductor.Owner = this;
                reproductor.ShowDialog(); // Modal
            }
            else
            {
                MessageBox.Show("Primero selecciona un video.");
            }*/

        }
    }
}
