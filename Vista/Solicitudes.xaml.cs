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
    /// Lógica de interacción para SolicitudesAdopcion.xaml
    /// </summary>
    public partial class Solicitudes : Window
    {
        public Solicitudes()
        {
            InitializeComponent();
            CargarSolicitudes();
        }

        private void CargarSolicitudes()
        {
            /*foreach (var solicitud in solicitudes)
            {
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Imagen de perfil circular
                var imagen = new Ellipse
                {
                    Width = 40,
                    Height = 40,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = new ImageBrush(new BitmapImage(new Uri("ruta/a/imagen.png", UriKind.RelativeOrAbsolute)))
                };
                panel.Children.Add(imagen);

                // Nombre del usuario
                var nombre = new TextBlock
                {
                    Text = solicitud.NombreCompleto,
                    Foreground = Brushes.White,
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 20, 0),
                    Width = 180
                };
                panel.Children.Add(nombre);

                // Botón Aceptar
                var btnAceptar = new Button
                {
                    Content = "✔",
                    Background = Brushes.MediumSeaGreen,
                    Foreground = Brushes.White,
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5, 0, 5, 0)
                };
                // btnAceptar.Click += ...
                panel.Children.Add(btnAceptar);

                // Botón Rechazar
                var btnRechazar = new Button
                {
                    Content = "✖",
                    Background = Brushes.IndianRed,
                    Foreground = Brushes.White,
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5, 0, 5, 0)
                };
                // btnRechazar.Click += ...
                panel.Children.Add(btnRechazar);

                // Botón de Mensaje
                var btnMensaje = new Button
                {
                    Content = "💬",
                    Background = Brushes.SkyBlue,
                    Foreground = Brushes.White,
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                // btnMensaje.Click += ...
                panel.Children.Add(btnMensaje);

                // Separador inferior
                var contenedor = new StackPanel();
                contenedor.Children.Add(panel);
                contenedor.Children.Add(new Separator { Margin = new Thickness(0, 5, 0, 5), Background = Brushes.White });

                SolicitudesPanel.Children.Add(contenedor);
            }*/
        }


        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
