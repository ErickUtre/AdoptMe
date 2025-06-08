using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Servicios
{
    public class AccesoServicios
    {
        private readonly HttpClient _httpClient;

        public AccesoServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<HttpResponseMessage> IniciarSesionAsync(string correo, string contrañaHash)
        {
            var datos = new
            {
                Correo = correo,
                ContrasenaHash = contrañaHash
            };

            string json = JsonConvert.SerializeObject(datos);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _httpClient.PostAsync(_httpClient.BaseAddress + "acceso/iniciar-sesion", contenido);
            return respuesta;
        }
    }
}
