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
    /// Lógica de interacción para RegistrarAdopcion.xaml
    /// </summary>
    public partial class RegistrarAdopcion : Page
    {
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
                //PROCESO DE REGISTRO EN LA BD
            }
        }
    }
}
