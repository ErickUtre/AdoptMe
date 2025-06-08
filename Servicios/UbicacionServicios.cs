using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_AdoptMe.Servicios
{
    public class UbicacionServicios
    {
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
    }
}
