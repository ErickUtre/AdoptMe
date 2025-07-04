using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
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
    /// Lógica de interacción para Notificaciones.xaml
    /// </summary>
    public partial class Notificaciones : Page
    {
        public Notificaciones()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarNotificacionesAsync();
            EscucharNuevasNotificaciones();
        }

        private async Task CargarNotificacionesAsync()
        {
            try
            {
                ListaNotificaciones.Items.Clear();
                NotificacionServicios notificacionServicios = new NotificacionServicios();
                string token = UsuarioSingleton.Instancia.Token;

                ResultadoHttp resultadoHttp = await notificacionServicios.ObtenerNotificacionesAsync(token);

                if (!resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                string respuesta = await resultadoHttp.Respuesta.Content.ReadAsStringAsync();

                var resultado = JsonConvert.DeserializeObject<List<Notificacion>>(respuesta);

                foreach (var notificacion in resultado)
                {
                    Console.WriteLine(notificacion.Fecha);
                    ListaNotificaciones.Items.Add(notificacion);
                }
            }
            catch (Exception ex)
            {
                Registro.Error("Error al cargar historial de notificaciones: " +  ex.Message);
                MessageBox.Show(
                        Properties.Resources.mensaje_ErrorGeneral,
                        Properties.Resources.global_Error,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }

        private void EscucharNuevasNotificaciones()
        {
            var servicio = UsuarioSingleton.Instancia.ServicioNotificacion;

            if (servicio != null)
            {
                servicio.NotificacionRecibida += noti =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Notificacion notificacion = new Notificacion
                        {
                            NotificacionId = noti.NotificacionId,
                            Titulo = noti.Titulo,
                            Mensaje = noti.Mensaje,
                            Tipo = noti.Tipo,
                            ReferenciaId = noti.ReferenciaId,
                            ReferenciaTipo = noti.ReferenciaTipo,
                            Fecha = DateTime.TryParse(noti.Fecha, out var fecha) ? fecha : DateTime.Now
                        };

                        ListaNotificaciones.Items.Insert(0, notificacion);
                    });
                };
            }
        }

        private void Btn_Notificacion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Notificacion notificacion)
            {
                if (notificacion.ReferenciaTipo == "Adopcion" && notificacion.ReferenciaId.HasValue)
                {
                    int adopcionId = notificacion.ReferenciaId.Value;

                    MapaPrincipal mapa = new MapaPrincipal(adopcionId);
                    NavegadorPrincipal.Instancia.Navegar(mapa);
                }
            }
        }

        private async void Btn_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Notificacion notificacion)
            {
                NotificacionServicios notificacionServicios = new NotificacionServicios();
                ResultadoHttp resultadoHttp = await notificacionServicios.EliminarNotificacion(UsuarioSingleton.Instancia.Token, notificacion.NotificacionId);

                if (!resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                await CargarNotificacionesAsync();
            }
        }

        private async void Btn_EliminarTodas_Click(object sender, RoutedEventArgs e)
        {
            NotificacionServicios notificacionServicios = new NotificacionServicios();
            ResultadoHttp resultadoHttp = await notificacionServicios.EliminarNotificaciones(UsuarioSingleton.Instancia.Token);

            if (!resultadoHttp.Exito)
            {
                MessageBox.Show(
                    resultadoHttp.MensajeError,
                    Properties.Resources.global_ErrorServidor,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            await CargarNotificacionesAsync();
        }

    }
}
