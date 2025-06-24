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
        private static readonly Dictionary<string, BitmapImage> _cacheFotosMascotas = new Dictionary<string, BitmapImage>();

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

        public static async Task<BitmapImage> ObtenerFotoMascotaAsync(int idMascota, string token, bool esMascotaPropia)
        {
            string cacheKey = $"fotoMascota_{idMascota}";

            if (esMascotaPropia && _cacheFotosMascotas.TryGetValue(cacheKey, out BitmapImage imagenCacheada))
            {
                return imagenCacheada;
            }

            try
            {
                MascotaServicios mascotaServicios = new MascotaServicios();
                var response = await mascotaServicios.SolicitarFotoMascotaAsync(idMascota, token);

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await mascotaServicios.ObtenerContenidoFotoMascotaAsync(response);
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

                        _cacheFotosMascotas[cacheKey] = imagen;

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

        public static async Task<MemoryStream> ObtenerVideoMascotaAsync(int idMascota, string token)
        {
            try
            {
                MascotaServicios mascotaServicios = new MascotaServicios();
                var response = await mascotaServicios.SolicitarVideoMascotaAsync(idMascota, token);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await mascotaServicios.ObtenerContenidoVideoMascotaAsync(response);
                    if (stream != null)
                    {
                        var ms = new MemoryStream();
                        await stream.CopyToAsync(ms);
                        ms.Position = 0;

                        return ms;
                    }
                }
                else
                {
                    Registro.Error($"Error al obtener el video: {response.StatusCode} - {response.Content}");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción al obtener video: {ex.Message}\nTraza: {ex.StackTrace}");
            }

            return null;
        }
    }
}
