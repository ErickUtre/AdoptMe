using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Cliente_AdoptMe.Vista
{
    public partial class ConsultarAdopciones : Page
    {
        private readonly AdopcionServicios _adopcionServicios;
        private List<Adopcion> _listaCompletaAdopciones;

        public ConsultarAdopciones()
        {
            InitializeComponent();
            _adopcionServicios = new AdopcionServicios();
            this.Loaded += ConsultarAdopciones_Loaded;
        }

        private async void ConsultarAdopciones_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarAdopcionesPorPublicadorAsync();
        }

        private async Task CargarAdopcionesPorPublicadorAsync()
        {
            try
            {
                int idPublicador = UsuarioSingleton.Instancia.UsuarioActual.UsuarioId;

                List<Adopcion> adopciones = await _adopcionServicios.ObtenerAdopcionesPorPublicadorAsync(idPublicador);

                if (adopciones == null || adopciones.Count == 0)
                {
                    MessageBox.Show("No se encontraron adopciones registradas.");
                    return;
                }

                foreach (var adopcion in adopciones)
                {
                    if (adopcion.Mascota == null) continue;

                    adopcion.EstadoTexto = "Disponible";
                    adopcion.ColorEstado = new SolidColorBrush(Colors.Green);
                    adopcion.Foto = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/MascotaDefault.png"));
                }

                _listaCompletaAdopciones = adopciones;
                listaAdopciones.ItemsSource = _listaCompletaAdopciones;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al cargar adopciones: " + ex.Message);
            }
        }

        private void TbNombreMascota_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = tbNombreMascota.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(filtro))
            {
                listaAdopciones.ItemsSource = _listaCompletaAdopciones;
            }
            else
            {
                var filtradas = _listaCompletaAdopciones
                    .Where(a => a.Mascota != null &&
                                a.Mascota.Nombre != null &&
                                a.Mascota.Nombre.ToLower().Contains(filtro))
                    .ToList();

                listaAdopciones.ItemsSource = filtradas;
            }
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var adopcionSeleccionada = button.DataContext as Adopcion;
                if (adopcionSeleccionada != null)
                {
                    ConsultarAdopcion paginaDetalle = new ConsultarAdopcion(adopcionSeleccionada);
                    NavigationService.Navigate(paginaDetalle);
                }
            }
        }
    }
}
