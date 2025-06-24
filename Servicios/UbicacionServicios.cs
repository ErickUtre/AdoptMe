using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace Cliente_AdoptMe.Servicios
{
    public class UbicacionServicios
    {
        private readonly HttpClient _httpClient;

        public UbicacionServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<Ubicacion> ObtenerUbicacionPorIPAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("http://ip-api.com/json");
                    JObject data = JObject.Parse(response);

                    string ciudad = data["city"]?.ToString();
                    string estado = data["regionName"]?.ToString();
                    string pais = data["country"]?.ToString();
                    string latitud = data["lat"]?.ToString();
                    string longitud = data["lon"]?.ToString();

                    Ubicacion ubicacion = new Ubicacion()
                    {
                        Ciudad = ciudad,
                        Estado = estado,
                        Pais = pais,
                        Latitud = double.Parse(latitud),
                        Longitud = double.Parse(longitud)
                    };


                    return ubicacion;
                }
            }
            catch (HttpRequestException)
            {
                return new Ubicacion() { UbicacionID = -1 };
            }
        }

        public async Task<Ubicacion> ObtenerDireccionPorCoordenadasAsync(double lat, double lon)
        {
            try
            {
                var url = $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AdoptMe/1.0");

                    var response = await client.GetStringAsync(url);
                    var json = JObject.Parse(response);

                    var address = json["address"];
                    string ciudad = address?["city"]?.ToString() ?? address?["town"]?.ToString() ?? address?["village"]?.ToString();
                    string estado = address?["state"]?.ToString();
                    string pais = address?["country"]?.ToString();

                    Ubicacion ubicacion = new Ubicacion()
                    {
                        Ciudad = ciudad,
                        Estado = estado,
                        Pais = pais,
                        Latitud = lat,
                        Longitud = lon
                    };

                    return ubicacion;
                }
            }
            catch (HttpRequestException)
            { 
                return new Ubicacion() { UbicacionID = -1 };
            }
        }

        public async Task<HttpResponseMessage> ObtenerSolicitudesCercanasAsync(double latitud, double longitud, string token, int radioMts = 5000)
        {
            string url = $"{_httpClient.BaseAddress}ubicaciones/cercanos?Latitud={latitud}&Longitud={longitud}&radio={radioMts}";

            var solicitud = new HttpRequestMessage(HttpMethod.Get, url);
            solicitud.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage respuesta = await _httpClient.SendAsync(solicitud);
            return respuesta;
        }

        public async Task<ResultadoHttp> ActualizarUbicacionAsync(Ubicacion ubicacion, string token)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(ubicacion);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.PutAsync("ubicaciones", contenido)
            );
        }
    }
}
