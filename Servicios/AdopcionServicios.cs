using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_AdoptMe.Servicios
{
    public class AdopcionServicios
    {
        private readonly HttpClient _httpClient;

        public AdopcionServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<HttpResponseMessage> RegistrarAdopcionAsync(Adopcion nuevaAdopcion)
        {
            string json = JsonConvert.SerializeObject(nuevaAdopcion);

            Debug.WriteLine("JSON enviado (registrar):");
            Debug.WriteLine(json);

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _httpClient.PostAsync("adopciones", contenido);
            return respuesta;
        }

        public async Task<List<Adopcion>> ObtenerAdopcionesPorPublicadorAsync(int publicadorId)
        {
            try
            {
                string url = $"adopciones/por-publicador/{publicadorId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var adopciones = JsonConvert.DeserializeObject<List<Adopcion>>(json);
                    return adopciones;
                }
                else
                {
                    return new List<Adopcion>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener adopciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Adopcion>();
            }
        }

        public async Task<ResultadoHttp> ObtenerAdopcionesPendientesAsync()
        {
            return await HttpHelper.EjecutarHttp(() =>
            {
                return _httpClient.GetAsync("adopciones/pendientes");
            });
        }

        public async Task<ResultadoHttp> ObtenerAdopcionesAceptadasAsync()
        {
            return await HttpHelper.EjecutarHttp(() =>
            {
                return _httpClient.GetAsync("adopciones/aceptadas");
            });
        }

        public async Task<HttpResponseMessage> ModificarAdopcionAsync(int idAdopcion, Adopcion adopcionModificada)
        {
            try
            {
                string url = $"adopciones/{idAdopcion}";

                string json = JsonConvert.SerializeObject(adopcionModificada);

                Debug.WriteLine("JSON enviado (modificar):");
                Debug.WriteLine(json);

                var contenido = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage respuesta = await _httpClient.PutAsync(url, contenido);
                return respuesta;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar adopción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> EliminarAdopcionAsync(int idAdopcion)
        {
            try
            {
                string url = $"adopciones/{idAdopcion}";
                HttpResponseMessage respuesta = await _httpClient.DeleteAsync(url);
                return respuesta;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar adopción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
