using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
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
using static Cliente_AdoptMe.Utilidades.InterfazUsuarioHelper;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarAdopcionExterna.xaml
    /// </summary>
    public partial class ConsultarAdopcionExterna : Page
    {
        UbicacionGrpc.Mascota _mascota;
        public ConsultarAdopcionExterna(UbicacionGrpc.Mascota mascota)
        {
            _mascota = mascota;
            InitializeComponent();
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

        private async Task MostrarFotoAsync()
        {
            var imagen = await ObtenerFotoMascotaAsync(_mascota.MascotaId, UsuarioSingleton.Instancia.Token, false);
            if (imagen != null)
            {
                FotoMascota.Source = imagen;
                FotoMascotaFondo.Source = imagen;
            }
        }

        private async void Btn_VerVideo(object sender, RoutedEventArgs e)
        {
            await MostrarVideoMascotaAsync(_mascota.MascotaId, UsuarioSingleton.Instancia.Token);
        }

        public async Task MostrarVideoMascotaAsync(int idMascota, string token)
        {
            var msVideo = await ObtenerVideoMascotaAsync(idMascota, token);
            if (msVideo != null)
            {
                string rutaTemporal = await GuardarVideoTemporalAsync(msVideo, idMascota);
                var ventanaVideo = new Video(rutaTemporal);
                ventanaVideo.Show();
            }
            else
            {
                MessageBox.Show("No se pudo descargar el video.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
