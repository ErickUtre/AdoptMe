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
    /// Lógica de interacción para EditarCampo.xaml
    /// </summary>
    public partial class EditarCampo : Window
    {
        public string NuevoValor { get; private set; }
        private string _tipoValor;

        public EditarCampo(string tipoValor)
        {
            InitializeComponent();
            _tipoValor = tipoValor;
        }

        private bool ValidarCampo()
        {
            string valor = Tb_NuevoValor.Text.Trim();
            bool esValido = false;

            switch (_tipoValor)
            {
                case var tipo when tipo == Properties.Resources.global_Nombre:
                    esValido = Validador.ValidarNombre(valor) &&
                        !valor.Equals(UsuarioSingleton.Instancia.UsuarioActual.Nombre);
                    break;
                case var tipo when tipo == Properties.Resources.global_Correo:
                    esValido = Validador.ValidarCorreo(valor) &&
                        !valor.Equals(UsuarioSingleton.Instancia.UsuarioActual.Acceso.Correo); 
                    break;
                case var tipo when tipo == Properties.Resources.global_Telefono:
                    esValido = Validador.ValidarTelefono(valor) &&
                        !valor.Equals(UsuarioSingleton.Instancia.UsuarioActual.Telefono);
                    break;
                default:
                    esValido = false;
                    break;
            }

            if (!esValido)
            {
                Tb_NuevoValor.BorderBrush = Brushes.Red;
                MessageBox.Show(
                    "Debes ingresar un valor válido y diferente al actual.", 
                    "Validación", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning
                );
            }
            else
            {
                Tb_NuevoValor.BorderBrush = Brushes.Transparent;
            }

            return esValido;
        }

        private void Btn_Guardar(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampo()) { return; }

            NuevoValor = Tb_NuevoValor.Text;
            this.DialogResult = true;  
            this.Close();
        }

        private void Btn_Cancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
