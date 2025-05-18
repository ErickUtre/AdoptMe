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
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para InicioDeSesion.xaml
    /// </summary>
    public partial class InicioDeSesion : Window
    {
        public InicioDeSesion()
        {
            InitializeComponent();
        }

        private void PbCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void BtnIrVentanaRegistro(object sender, RoutedEventArgs e)
        {
            RegistroDeUsuario ventana = new RegistroDeUsuario();
            this.Hide();
            ventana.ShowDialog();
            this.Show();
        }

        private void BtnIniciarSesion(object sender, RoutedEventArgs e)
        {

        }
    }
}
