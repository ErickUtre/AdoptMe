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
    public class SolicitudServicios
    {
        private readonly HttpClient _httpClient;

        public SolicitudServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<List<Solicitud>> ObtenerSolicitudesConNombresPorAdopcionIDAsync(int adopcionID)
        {
            try
            {
                string url = $"solicitudes/adoptante/{adopcionID}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("JSON recibido (solicitudes):");
                    Debug.WriteLine(json);

                    var solicitudes = JsonConvert.DeserializeObject<List<Solicitud>>(json);
                    return solicitudes ?? new List<Solicitud>();
                }
                else
                {
                    return new List<Solicitud>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener solicitudes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Solicitud>();
            }
        }

        public async Task<bool> EliminarSolicitudAsync(int solicitudId)
        {
            try
            {
                string url = $"solicitudes/{solicitudId}";
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Solicitud {solicitudId} eliminada correctamente.");
                    return true;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error al eliminar solicitud: {response.StatusCode} - {error}");
                    MessageBox.Show($"No se pudo eliminar la solicitud.\nCódigo: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<ResultadoHttp> RegistrarSolicitudAsync(int AdopcionID, string token)
        {
            var cuerpo = new { AdopcionID };
            var json = System.Text.Json.JsonSerializer.Serialize(cuerpo);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.PostAsync("solicitudes", contenido)
            );
        }
    }
}
