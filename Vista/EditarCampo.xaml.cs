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
        public string nuevoValor { get; private set; }

        public EditarCampo()
        {
            InitializeComponent();
        }

        private void Btn_Guardar(object sender, RoutedEventArgs e)
        {
            nuevoValor = Tb_NuevoValor.Text;
            this.DialogResult = true;  
            this.Close();
        }

        private void Btn_Cancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
