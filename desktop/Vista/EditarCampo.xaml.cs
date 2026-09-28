using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cliente_AdoptMe.Vista
{
    public partial class EditarCampo : Window
    {
        public string NuevoValor { get; private set; }
        private string _atributoAModificar;
        private Adopcion _adopcion;
        private AdopcionServicios _adopcionServicios = new AdopcionServicios();

        public EditarCampo(string atributoAModificar)
        {
            InitializeComponent();
            _atributoAModificar = atributoAModificar;
            _adopcion = null;
            ConfigurarInterfaz();
        }

        public EditarCampo(string atributoAModificar, Adopcion adopcion)
        {
            InitializeComponent();
            _atributoAModificar = atributoAModificar;
            _adopcion = adopcion;
            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            Tb_NuevoValor.Visibility = Visibility.Collapsed;
            Cb_Sexo.Visibility = Visibility.Collapsed;
            PanelEdad.Visibility = Visibility.Collapsed;

            switch (_atributoAModificar.ToLower())
            {
                case "sexo":
                    Cb_Sexo.Visibility = Visibility.Visible;
                    break;
                case "edad":
                    PanelEdad.Visibility = Visibility.Visible;
                    break;
                case "tamaño":
                    Tb_NuevoValor.Visibility = Visibility.Visible;
                    Tb_NuevoValor.MaxLength = 3;
                    Tb_NuevoValor.PreviewTextInput += (s, e) =>
                    {
                        e.Handled = !int.TryParse(e.Text, out _);
                    };
                    break;
                default:
                    Tb_NuevoValor.Visibility = Visibility.Visible;
                    break;
            }
        }

        private bool ValidarCampo()
        {
            string valor = "";

            switch (_atributoAModificar.ToLower())
            {
                case "sexo":
                    if (Cb_Sexo.SelectedItem == null)
                    {
                        MessageBox.Show("Seleccione una opción válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    valor = ((ComboBoxItem)Cb_Sexo.SelectedItem).Content.ToString();
                    if (valor == "Sexo")
                    {
                        MessageBox.Show("Seleccione una opción válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    break;

                case "edad":
                    if (Cb_Anios.SelectedItem == null || Cb_Meses.SelectedItem == null)
                    {
                        MessageBox.Show("Seleccione año y mes.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    string aniosStr = ((ComboBoxItem)Cb_Anios.SelectedItem).Content.ToString();
                    string mesesStr = ((ComboBoxItem)Cb_Meses.SelectedItem).Content.ToString();

                    if (aniosStr == "Año" || mesesStr == "Mes")
                    {
                        MessageBox.Show("Seleccione un valor válido para año y mes.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    valor = $"{aniosStr} año(s) con {mesesStr} mes(es)";
                    break;

                case "tamaño":
                    valor = Tb_NuevoValor.Text.Trim();
                    if (!int.TryParse(valor, out int numero) || numero < 0 || numero > 300)
                    {
                        MessageBox.Show("Ingrese un número entre 0 y 300.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    break;

                default:
                    valor = Tb_NuevoValor.Text.Trim();
                    if (string.IsNullOrEmpty(valor))
                    {
                        MessageBox.Show("No puede estar vacío.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    break;
            }

            NuevoValor = valor;
            return true;
        }


        private async void Btn_Guardar(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampo()) return;

            if (_adopcion?.Mascota == null)
            {
                this.DialogResult = true;
                this.Close();
                return;
            }

            switch (_atributoAModificar.ToLowerInvariant())
            {
                case "nombre":
                    _adopcion.Mascota.Nombre = NuevoValor;
                    break;
                case "especie":
                    _adopcion.Mascota.Especie = NuevoValor;
                    break;
                case "raza":
                    _adopcion.Mascota.Raza = NuevoValor;
                    break;
                case "edad":
                    _adopcion.Mascota.Edad = NuevoValor;
                    break;
                case "sexo":
                    _adopcion.Mascota.Sexo = NuevoValor;
                    break;
                case "tamaño":
                    _adopcion.Mascota.Tamaño = NuevoValor;
                    break;
                case "descripcion":
                    _adopcion.Mascota.Descripcion = NuevoValor;
                    break;
                default:
                    MessageBox.Show("Atributo no reconocido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            try
            {
                var response = await _adopcionServicios.ModificarAdopcionAsync(_adopcion.AdopcionID, _adopcion);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Campo actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Error al actualizar: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_Cancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
