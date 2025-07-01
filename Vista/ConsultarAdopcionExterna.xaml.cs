using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cliente_AdoptMe.Vista
{
    public partial class ConsultarAdopcionExterna : Page
    {
        private UbicacionGrpc.Mascota _mascota;
        private int _adopcionId;
        private int _publicadorId;

        public ConsultarAdopcionExterna(UbicacionGrpc.Mascota mascota, int adopcionId)
        {
            _mascota = mascota;
            _adopcionId = adopcionId;
            InitializeComponent();

            if (UsuarioSingleton.Instancia.UsuarioActual.Acceso.EsAdmin)
            {
                Btn_Solicitar.Visibility = Visibility.Collapsed;
            }

            InicializarDatos();
        }

        public async void InicializarDatos()
        {
            txtbl_Nombre.Text += $": {_mascota.Nombre}";
            txtbl_Especie.Text += $": {_mascota.Especie}";
            txtbl_Raza.Text += $": {_mascota.Raza}";
            txtbl_Edad.Text += $": {_mascota.Edad}";
            txtbl_Sexo.Text += $": {_mascota.Sexo}";
            txtbl_Estatura.Text += $": {_mascota.Tamano}";
            txtbl_Descripcion.Text = _mascota.Descripcion;

            await MostrarFotoAsync();
            await CargarDatosAdopcion();
        }

        private async Task MostrarFotoAsync()
        {
            var imagen = await InterfazUsuarioHelper.ObtenerFotoMascotaAsync(_mascota.MascotaId, UsuarioSingleton.Instancia.Token, false);
            if (imagen != null)
            {
                FotoMascota.Source = imagen;
                FotoMascotaFondo.Source = imagen;
            }
        }

        private async Task CargarDatosAdopcion()
        {
            try
            {
                var adopcionServicios = new AdopcionServicios();
                Adopcion adopcion = await adopcionServicios.ObtenerAdopcionPorIdAsync(_adopcionId);

                if (adopcion != null)
                {
                    _publicadorId = adopcion.PublicadorID;
                }
                else
                {
                    MessageBox.Show("No se encontró la adopción.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la adopción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            NavegadorPrincipal.Instancia.Navegar(new MapaPrincipal());
        }

        private void BtnExpandirImagen(object sender, RoutedEventArgs e)
        {
            ImagenExpandida imagenExpandida = new ImagenExpandida(FotoMascota);
            imagenExpandida.ShowDialog();
        }

        private async void Btn_VerVideo(object sender, RoutedEventArgs e)
        {
            await MostrarVideoMascotaAsync(_mascota.MascotaId, UsuarioSingleton.Instancia.Token);
        }

        public async Task MostrarVideoMascotaAsync(int idMascota, string token)
        {
            var msVideo = await InterfazUsuarioHelper.ObtenerVideoMascotaAsync(idMascota, token);
            if (msVideo != null)
            {
                string rutaTemporal = await GuardarVideoTemporalAsync(msVideo, idMascota);
                var ventanaVideo = new Video(rutaTemporal);
                ventanaVideo.ShowDialog();
            }
            else
            {
                MessageBox.Show("No hay video disponible", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task<string> GuardarVideoTemporalAsync(MemoryStream ms, int idMascota)
        {
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"video_mascota_{idMascota}.mp4");

            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                ms.Position = 0;
                await ms.CopyToAsync(fileStream);
            }

            return tempFile;
        }

        private async void BtnSolicitar(object sender, RoutedEventArgs e)
        {
            SolicitudServicios solicitudServicios = new SolicitudServicios();

            ResultadoHttp resultadoHttp = await solicitudServicios.RegistrarSolicitudAsync(
                _adopcionId,
                UsuarioSingleton.Instancia.Token
            );

            if (resultadoHttp.Exito)
            {
                MessageBox.Show("Solicitud enviada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (resultadoHttp.Codigo == System.Net.HttpStatusCode.Conflict)
            {
                MessageBox.Show("Ya has enviado una solicitud para esta adopción", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show(resultadoHttp.MensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_EnviarMensaje(object sender, RoutedEventArgs e)
        {
            if (_publicadorId == 0)
            {
                MessageBox.Show("No se pudo identificar al publicador.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavegadorPrincipal.Instancia.Navegar(new Chat(_publicadorId));
        }
    }
}
