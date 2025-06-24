using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _httpClient.PostAsync(_httpClient.BaseAddress + "adopciones", contenido);
            return respuesta;
        }
    }
}
