using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
                    MessageBox.Show($"No se pudieron obtener las solicitudes. Código: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return new List<Solicitud>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener solicitudes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Solicitud>();
            }
        }
    }
}
