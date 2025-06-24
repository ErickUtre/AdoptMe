using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cliente_AdoptMe.Vista
{
    public partial class Solicitudes : Window
    {
        private readonly SolicitudServicios _solicitudServicios;
        private readonly AdopcionServicios _adopcionServicios;
        private int _adopcionID;

        public Solicitudes(int adopcionID)
        {
            InitializeComponent();

            _solicitudServicios = new SolicitudServicios();
            _adopcionServicios = new AdopcionServicios();
            _adopcionID = adopcionID;

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
                        Width = 250,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    stack.Children.Add(nombreText);

                    var btnAceptar = new Button
                    {
                        Content = "Aceptar",
                        Width = 80,
                        Height = 25,
                        Margin = new Thickness(15, 0, 5, 0),
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
                var result = MessageBox.Show($"¿Estás seguro de que deseas rechazar la solicitud de {solicitud.NombreAdoptante}?", "Confirmar rechazo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool eliminado = await _solicitudServicios.EliminarSolicitudAsync(solicitud.SolicitudID);
                    if (eliminado)
                    {
                        if (btn.Parent is Panel stack && stack.Parent is Border border)
                        {
                            SolicitudesPanel.Children.Remove(border);
                        }
                        MessageBox.Show("Solicitud rechazada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private async void AceptarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Solicitud solicitud)
            {
                var result = MessageBox.Show($"¿Confirmas aceptar la solicitud de {solicitud.NombreAdoptante}?", "Confirmar aceptación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool eliminado = await _solicitudServicios.EliminarSolicitudAsync(solicitud.SolicitudID);
                    if (!eliminado)
                    {
                        MessageBox.Show("No se pudo eliminar la solicitud. Operación cancelada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var adopcionModificada = new Adopcion
                    {
                        Estado = true
                    };

                    var response = await _adopcionServicios.ModificarAdopcionAsync(_adopcionID, adopcionModificada);

                    if (response.IsSuccessStatusCode)
                    {
 
                        if (btn.Parent is Panel stack && stack.Parent is Border border)
                        {
                            SolicitudesPanel.Children.Remove(border);
                        }

                        MessageBox.Show("Solicitud aceptada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar la adopción después de aceptar la solicitud.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
