using Cliente_AdoptMe.Utilidades;
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
    /// Lógica de interacción para ConsultarAdopcionExterna.xaml
    /// </summary>
    public partial class ConsultarAdopcionExterna : Page
    {
        public ConsultarAdopcionExterna()
        {
            InitializeComponent();

            //VALIDAR SI ES ADMIN O NO
            /*if ()
            {
                Btn_EnviarMensaje.Visibility = Visibility.Collapsed;
                Btn_Solicitar.Visibility = Visibility.Collapsed;
                Btn_Cancelar.Visibility = Visibility.Collapsed;
            }*/
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
    }
}
