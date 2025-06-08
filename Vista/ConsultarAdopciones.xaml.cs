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
using Cliente_AdoptMe.Modelo;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarAdopciones.xaml
    /// </summary>
    public partial class ConsultarAdopciones : Page
    {
        private Mascota[] mascotas = new Mascota[1000];
        public ConsultarAdopciones()
        {
            InitializeComponent();
            Inicializar_Adopciones();

        }

        private void Inicializar_Adopciones()
        {
            //LOGICA PARA TRAER LAS ADOPCIONES A LA BASE DE DATOS

            //SE MAPEAN
            mascotas[mascotas.Length - 1] = new Mascota
            {
                Nombre = "Bongo",
                Especie = "Perro",
                Raza = "Dalmata",
                Edad = "3 años con 2 meses"
            };

            //SE MUESTRAN EN LA INTERFAZ GRÁFICA COMO ELEMENTOS
            Tb_Nombre.Text += mascotas[mascotas.Length - 1].Nombre;
            Tb_Especie.Text += mascotas[mascotas.Length - 1].Especie;
            Tb_Raza.Text += mascotas[mascotas.Length - 1].Raza;
            Tb_Edad.Text += mascotas[mascotas.Length - 1].Edad;
            Foto.Source = new BitmapImage(new Uri("pack://application:,,,/Recursos/Imagenes/Bongo.png"));
            /*
            if (mascotas[mascotas.Length - 1].EstadoAdopcion)
            {
                Tb_Estado.Text += "Sin adoptar";
                Ell_Estado.Fill = new SolidColorBrush(Colors.Green);

            }
            else
            {
                Tb_Estado.Text += "Adoptado/a";
                Ell_Estado.Fill = new SolidColorBrush(Colors.Red);
            }
            */
        }

        private void Btn_SolicitudesPendientes(object sender, RoutedEventArgs e)
        {
            SolicitudesAdopcion solicitudesAdopcion = new SolicitudesAdopcion();
            solicitudesAdopcion.ShowDialog();
        }

        private void Btn_Eliminar(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Consultar(object sender, RoutedEventArgs e)
        {
            //SI HACES CLIC AL ELEMENTO EN CUESTION LO ASOCIA CON SU ID Y SU INFORMACIÓN
            NavegadorPrincipal.Instancia.Navegar(new ConsultarAdopcion(mascotas[mascotas.Length - 1]));
        }
    }
}
