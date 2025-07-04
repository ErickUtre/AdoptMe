using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.SocketCliente;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cliente_AdoptMe.Vista
{
    public partial class Chat : Page
    {
        private readonly ChatServicios _chatServicios = new ChatServicios();
        private readonly UsuarioServicios _usuarioServicios = new UsuarioServicios();
        private readonly MenuPrincipalUsuario _menuPrincipalUsuario = NavegadorPrincipal.Instancia.GetVentanaContenedora<MenuPrincipalUsuario>();

        public ObservableCollection<MensajeUI> MensajesUI { get; set; } = new ObservableCollection<MensajeUI>();

        private int UsuarioActualID;
        private int UsuarioDestinoID;

        public Chat(int usuarioDestinoID)
        {
            InitializeComponent();
            DataContext = this;

            UsuarioActualID = UsuarioSingleton.Instancia.UsuarioActual.UsuarioId;
            UsuarioDestinoID = usuarioDestinoID;

            _menuPrincipalUsuario?.MostrarOverlay();
            CargarNombreUsuarioDestino();
            CargarMensajesAsync();
            _menuPrincipalUsuario.OcultarOverlay();

            ConectarSocket();

            this.Unloaded += (s, e) =>
            {
                SocketCliente.SocketCliente.MensajeRecibido -= OnNuevoMensajeRecibido;
            };
        }

        private async void CargarNombreUsuarioDestino()
        {
            try
            {
                var respuesta = await _usuarioServicios.ObtenerUsuarioPorIDAsync(UsuarioDestinoID);

                if (respuesta != null && !string.IsNullOrWhiteSpace(respuesta.Nombre))
                {
                    TxtTituloChat.Text = respuesta.Nombre;
                }
                else
                {
                    TxtTituloChat.Text = "Usuario";
                }
            }
            catch (Exception ex)
            {
                TxtTituloChat.Text = "Usuario";
                Registro.Error("Error al obtener nombre del usuario destino: " + ex.Message);
            }
        }

        private async void CargarMensajesAsync()
        {
            try
            {
                string token = UsuarioSingleton.Instancia.Token;
                var mensajes = await _chatServicios.ObtenerMensajesEntreUsuariosAsync(UsuarioActualID, UsuarioDestinoID, token);

                if (mensajes != null)
                {
                    MensajesUI.Clear();
                    foreach (var mensaje in mensajes.OrderBy(m => m.FechaEnvio))
                    {
                        MensajesUI.Add(new MensajeUI(mensaje, UsuarioActualID));
                    }
                    ScrollAlFinal();
                }
                else
                {
                    MessageBox.Show("No se pudieron cargar los mensajes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mensajes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConectarSocket()
        {
            if (!SocketCliente.SocketCliente.EstaConectado)
            {
                SocketCliente.SocketCliente.Conectar(UsuarioActualID);
            }

            SocketCliente.SocketCliente.MensajeRecibido += OnNuevoMensajeRecibido;
        }

        private void OnNuevoMensajeRecibido(object data)
        {
            try
            {
                string json = data.ToString();
                var mensaje = JsonConvert.DeserializeObject<Mensaje>(json);

                if ((mensaje.RemitenteID == UsuarioDestinoID && mensaje.DestinatarioID == UsuarioActualID) ||
                    (mensaje.RemitenteID == UsuarioActualID && mensaje.DestinatarioID == UsuarioDestinoID))
                {
                    Dispatcher.Invoke(() =>
                    {
                        MensajesUI.Add(new MensajeUI(mensaje, UsuarioActualID));
                        ScrollAlFinal();
                    });
                }
            }
            catch
            {
                // Ignorar errores
            }
        }

        private void ScrollAlFinal()
        {
            if (ScrollMensajes != null && ScrollMensajes.ScrollableHeight > 0)
            {
                ScrollMensajes.ScrollToEnd();
            }
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            EnviarMensaje();
        }

        private void TxtMensaje_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                EnviarMensaje();
                e.Handled = true;
            }
        }

        private void EnviarMensaje()
        {
            string texto = TxtMensaje.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;

            SocketCliente.SocketCliente.EnviarMensaje(UsuarioActualID, UsuarioDestinoID, texto);

            var mensajeUI = new MensajeUI
            {
                Contenido = texto,
                RemitenteID = UsuarioActualID,
                FechaEnvio = DateTime.Now,
                EsPropio = true
            };

            MensajesUI.Add(mensajeUI);
            ScrollAlFinal();
            TxtMensaje.Clear();
        }
    }
}
