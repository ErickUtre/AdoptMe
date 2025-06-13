using Cliente_AdoptMe.Grpc;
using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para Chat.xaml
    /// </summary>
    public partial class Chat : Page
    {
        private readonly ServicioMensajeGrpc _servicioMensaje;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Usuario _receptor;

        public Chat(Usuario receptor)
        {
            InitializeComponent();
            _receptor = receptor;
            _servicioMensaje = new ServicioMensajeGrpc();

            this.Unloaded += Chat_Unloaded;

            _ = EscucharMensajesAsync();
        }

        private async Task EscucharMensajesAsync()
        {
            await _servicioMensaje.SuscribirseMensajesAsync(mensaje =>
            {
                Dispatcher.Invoke(() =>
                {
                    lbMensajes.Items.Add($"[{mensaje.RemitenteID} -> {mensaje.ReceptorID}]: {mensaje.Contenido}");
                });
            }, _cts.Token);
        }

        private async void EnviarMensaje_Click(object sender, RoutedEventArgs e)
        {
            string texto = tbMensaje.Text.Trim();
            if (!string.IsNullOrEmpty(texto))
            {
                await _servicioMensaje.PublicarMensajeAsync(UsuarioSingleton.Instancia.UsuarioActual.UsuarioId, _receptor.UsuarioId, contenido: texto);
                tbMensaje.Clear();
            }
        }

        private async void Regresar_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            await _servicioMensaje.CerrarConexionAsync();
            NavegadorPrincipal.Instancia.Regresar();
        }

        private async void Chat_Unloaded(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            await _servicioMensaje.CerrarConexionAsync();
        }
    }
}
