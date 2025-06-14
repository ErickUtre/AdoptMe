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
    public class UsuarioServicios
    {
        private readonly HttpClient _httpClient;

        public UsuarioServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Usuario>>("usuarios");
        }

        public async Task<HttpResponseMessage> RegistrarUsuarioAsync(Usuario nuevoUsuario)
        {
            string json = JsonConvert.SerializeObject(nuevoUsuario);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _httpClient.PostAsync(_httpClient.BaseAddress + "usuarios", contenido);
            return respuesta;
        }

        public async Task<HttpResponseMessage> SolicitarFotoPerfil(string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "usuarios/foto-perfil"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return response;
            }
        }

        public async Task<byte[]> ObtenerContenidoFotoPerfil(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }
    }
}
