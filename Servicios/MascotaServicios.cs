using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Servicios
{
    public class MascotaServicios
    {
        private readonly HttpClient _httpClient;

        public MascotaServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<HttpResponseMessage> SolicitarFotoMascotaAsync(int idMascota, string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"mascotas/{idMascota}/foto"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return response;
            }
        }

        public async Task<byte[]> ObtenerContenidoFotoMascotaAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<HttpResponseMessage> SolicitarVideoMascotaAsync(int idMascota, string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"mascotas/{idMascota}/video"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return response;
            }
        }

        public async Task<Stream> ObtenerContenidoVideoMascotaAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            return null;
        }

    }
}
