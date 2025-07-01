using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cliente_AdoptMe.Vista
{
    public partial class Mensajes : Page
    {
        private readonly ChatServicios _chatServicios;
        public ObservableCollection<Chat> ListaChats { get; set; }

        public Mensajes()
        {
            InitializeComponent();
            _chatServicios = new ChatServicios();
            ListaChats = new ObservableCollection<Chat>();
            DataContext = this;

            Loaded += Mensajes_Loaded;
        }

        private async void Mensajes_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarChatsAsync();
        }

        private async Task CargarChatsAsync()
        {
            try
            {
                int usuarioID = UsuarioSingleton.Instancia.UsuarioActual.UsuarioId;
                string token = UsuarioSingleton.Instancia.Token;

                var _chatServicios = new ChatServicios();
                List<Modelo.Chat> chats = await _chatServicios.ObtenerChatsPorUsuarioAsync(usuarioID, token);

                ListViewChats.ItemsSource = chats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los chats: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListViewChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewChats.SelectedItem is Modelo.Chat chatSeleccionado)
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new Chat(chatSeleccionado.UsuarioID));
                }
                else
                {
                    MessageBox.Show("No se puede navegar, NavigationService es null.");
                }
            }
        }

    }
}
