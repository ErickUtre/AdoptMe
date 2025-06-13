using Cliente_AdoptMe.Grpc;
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
    /// Lógica de interacción para Mensajes.xaml
    /// </summary>
    public partial class Mensajes : Page
    {
        public Mensajes()
        {
            InitializeComponent();
            CargarContactos();
        }

        private async void CargarContactos()
        {
            ServicioMensajeGrpc servicioMensajeGrpc = new ServicioMensajeGrpc();
            var contactos = await servicioMensajeGrpc.ObtenerContactosAsync(UsuarioSingleton.Instancia.UsuarioActual.UsuarioId);

            foreach (var contacto in contactos)
            {
                lbContactos.Items.Add(contacto);
            }
        }
    }
}