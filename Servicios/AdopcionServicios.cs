using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

            Debug.WriteLine("JSON enviado:");
            Debug.WriteLine(json);

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _httpClient.PostAsync(_httpClient.BaseAddress + "adopciones", contenido);
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
    }
}
