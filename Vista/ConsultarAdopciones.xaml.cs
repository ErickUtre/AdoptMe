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
using static Cliente_AdoptMe.Utilidades.InterfazUsuarioHelper;

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
                    adopcion.Foto = await ObtenerFotoMascotaAsync(adopcion.MascotaID, UsuarioSingleton.Instancia.Token, true);
                }

                _listaCompletaAdopciones = adopciones;
                listaAdopciones.ItemsSource = _listaCompletaAdopciones;
            }
            catch (Exception ex)
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

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var adopcionParaEliminar = button.DataContext as Adopcion;
            if (adopcionParaEliminar == null) return;

            var resultado = MessageBox.Show(
                $"¿Seguro que deseas eliminar la adopción de '{adopcionParaEliminar.Mascota?.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var response = await _adopcionServicios.EliminarAdopcionAsync(adopcionParaEliminar.AdopcionID);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Adopción eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    _listaCompletaAdopciones.Remove(adopcionParaEliminar);

                    listaAdopciones.ItemsSource = null;
                    listaAdopciones.ItemsSource = _listaCompletaAdopciones;
                }
                else
                {
                    MessageBox.Show($"Error al eliminar adopción: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al eliminar adopción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSolicitudesPendientes_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var adopcion = button.DataContext as Adopcion;
            if (adopcion == null) return;

            var ventanaSolicitudes = new Solicitudes(adopcion.AdopcionID);
            ventanaSolicitudes.ShowDialog(); 
        }
    }
}
