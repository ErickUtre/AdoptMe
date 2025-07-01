using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Vista;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Servicios
{
    public class ChatServicios
    {
        private readonly HttpClient _httpClient;

        public ChatServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        // ✅ Obtener lista de chats del usuario autenticado
        public async Task<List<Modelo.Chat>> ObtenerChatsPorUsuarioAsync(int usuarioID, string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"chat/usuario/{usuarioID}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error al obtener chats: {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Modelo.Chat>>(json) ?? new List<Modelo.Chat>();
            }
        }

        // ✅ Obtener historial de mensajes entre dos usuarios
        public async Task<List<Mensaje>> ObtenerMensajesEntreUsuariosAsync(int usuarioA, int usuarioB, string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"chat/entre/{usuarioA}/{usuarioB}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error al obtener mensajes: {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Mensaje>>(json) ?? new List<Mensaje>();
            }
        }

        // ✅ Enviar nuevo mensaje
        public async Task<bool> EnviarMensajeAsync(int remitenteID, int destinatarioID, string contenido, string token)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "chat/"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var mensajeData = new
                {
                    RemitenteID = remitenteID,
                    DestinatarioID = destinatarioID,
                    Contenido = contenido
                };

                string json = JsonConvert.SerializeObject(mensajeData);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
