using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Vista;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cliente_AdoptMe.Utilidades
{
    public static class InterfazUsuarioHelper
    {
        public static void ReiniciarBordesTextBox(IEnumerable<TextBox> textBoxes, Brush color)
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.BorderBrush = color;
            }
        }

        public static void ReiniciarBordesPasswordBox(IEnumerable<PasswordBox> passwordBoxes, Brush color)
        {
            foreach (PasswordBox passwordBox in passwordBoxes)
            {
                passwordBox.BorderBrush = color;
            }
        }

        public static async Task<BitmapImage> ObtenerFotoPerfilAsync(string token)
        {
            try
            {
                UsuarioServicios usuarioServicios = new UsuarioServicios();
                var response = await usuarioServicios.SolicitarFotoPerfilAsync(token);

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await usuarioServicios.ObtenerContenidoFotoPerfilAsync(response);
                    if (bytes != null && bytes.Length > 0)
                    {
                        BitmapImage imagen = new BitmapImage();
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            imagen.BeginInit();
                            imagen.CacheOption = BitmapCacheOption.OnLoad;
                            imagen.StreamSource = ms;
                            imagen.EndInit();
                            imagen.Freeze();
                        }

                        return imagen;
                    }
                }
                else
                {
                    Registro.Error($"Error al obtener la foto: {response.StatusCode} - {response.Content}");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción al obtener foto: {ex.Message}\nTraza: {ex.StackTrace}");
            }

            return null;
        }
    }
}
