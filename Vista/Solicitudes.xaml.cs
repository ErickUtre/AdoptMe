using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;

namespace Cliente_AdoptMe.Vista
{
    public partial class Solicitudes : Window
    {
        private readonly SolicitudServicios _solicitudServicios;
        private readonly AdopcionServicios _adopcionServicios;
        private readonly ConsultarAdopciones _paginaPadre;
        private int _adopcionID;

        public Solicitudes(int adopcionID, ConsultarAdopciones paginaPadre)
        {
            InitializeComponent();
            _solicitudServicios = new SolicitudServicios();
            _adopcionServicios = new AdopcionServicios();
            _adopcionID = adopcionID;
            _paginaPadre = paginaPadre;

            CargarSolicitudes();
        }

        private async void CargarSolicitudes()
        {
            try
            {
                var solicitudes = await _solicitudServicios.ObtenerSolicitudesConNombresPorAdopcionIDAsync(_adopcionID);
                SolicitudesPanel.Children.Clear();

                if (solicitudes == null || solicitudes.Count == 0)
                {
                    SolicitudesPanel.Children.Add(new TextBlock
                    {
                        Text = "No hay solicitudes pendientes para esta adopción.",
                        Foreground = Brushes.White,
                        FontSize = 16,
                        Margin = new Thickness(5)
                    });
                    return;
                }

                foreach (var solicitud in solicitudes)
                {
                    Border border = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(169, 169, 169)),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(10),
                        Margin = new Thickness(5, 0, 5, 10),
                        Effect = new DropShadowEffect
                        {
                            Color = Colors.Black,
                            Direction = 320,
                            ShadowDepth = 3,
                            Opacity = 0.3,
                            BlurRadius = 5
                        }
                    };

                    var stack = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    var nombreText = new TextBlock
                    {
                        Text = solicitud.NombreAdoptante,
                        Foreground = Brushes.White,
                        FontSize = 16,
                        Width = 200,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    stack.Children.Add(nombreText);

                    var btnMensaje = new Button
                    {
                        Content = "Mandar mensaje",
                        Width = 120,
                        Height = 25,
                        Margin = new Thickness(5, 0, 5, 0),
                        Background = new SolidColorBrush(Color.FromRgb(70, 130, 180)),
                        Foreground = Brushes.White,
                        Tag = solicitud
                    };
                    btnMensaje.Click += MandarMensaje_Click;
                    stack.Children.Add(btnMensaje);

                    var btnAceptar = new Button
                    {
                        Content = "Aceptar",
                        Width = 80,
                        Height = 25,
                        Margin = new Thickness(5, 0, 5, 0),
                        Background = new SolidColorBrush(Color.FromRgb(100, 204, 32)),
                        Foreground = Brushes.White,
                        Tag = solicitud
                    };
                    btnAceptar.Click += AceptarSolicitud_Click;
                    stack.Children.Add(btnAceptar);

                    var btnRechazar = new Button
                    {
                        Content = "Rechazar",
                        Width = 80,
                        Height = 25,
                        Margin = new Thickness(5, 0, 5, 0),
                        Background = new SolidColorBrush(Color.FromRgb(234, 89, 89)),
                        Foreground = Brushes.White,
                        Tag = solicitud
                    };
                    btnRechazar.Click += RechazarSolicitud_Click;
                    stack.Children.Add(btnRechazar);

                    border.Child = stack;
                    SolicitudesPanel.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar solicitudes: " + ex.Message);
            }
        }

        private async void RechazarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Solicitud solicitud)
            {
                var result = MessageBox.Show($"¿Deseas rechazar la solicitud de {solicitud.NombreAdoptante}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool eliminado = await _solicitudServicios.EliminarSolicitudAsync(solicitud.SolicitudID);
                    if (eliminado && btn.Parent is Panel stack && stack.Parent is Border border)
                    {
                        SolicitudesPanel.Children.Remove(border);
                        MessageBox.Show("Solicitud rechazada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private async void AceptarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Solicitud solicitud)
            {
                var result = MessageBox.Show($"¿Aceptar la solicitud de {solicitud.NombreAdoptante} y marcar la adopción como adoptada?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool eliminado = await _solicitudServicios.EliminarSolicitudAsync(solicitud.SolicitudID);
                    if (!eliminado)
                    {
                        MessageBox.Show("Error al eliminar la solicitud.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var adopcionModificada = new Adopcion { Estado = true };
                    var response = await _adopcionServicios.ModificarAdopcionAsync(_adopcionID, adopcionModificada);

                    if (response.IsSuccessStatusCode)
                    {
                        if (btn.Parent is Panel stack && stack.Parent is Border border)
                        {
                            SolicitudesPanel.Children.Remove(border);
                        }

                        _paginaPadre.RefrescarAdopcion(_adopcionID);

                        MessageBox.Show("Solicitud aceptada y adopción actualizada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar adopción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void MandarMensaje_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Solicitud solicitud)
            {
                this.Close();
                NavegadorPrincipal.Instancia.Navegar(new Chat(solicitud.AdoptanteID));
            }
        }



        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
