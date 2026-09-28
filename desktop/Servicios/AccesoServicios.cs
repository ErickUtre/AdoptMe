using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task<ResultadoHttp> IniciarSesionAsync(string correo, string contrañaHash)
        {
            var datos = new
            {
                Correo = correo,
                ContrasenaHash = contrañaHash
            };

            string json = JsonConvert.SerializeObject(datos);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            return await HttpHelper.EjecutarHttp(() =>
            {   
                return _httpClient.PostAsync("acceso/iniciar-sesion", contenido);
            });
        }

        public async Task<ResultadoHttp> ActualizarAccesoAsync(string correo, string token)
        {
            var dato = new Dictionary<string, object>
            {
                { "Correo", correo }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(dato);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.PatchAsJsonAsync("acceso", contenido)
            );
        }
    }
}
