using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Servicios
{
    public class NotificacionServicios
    {
        private readonly HttpClient _httpClient;

        public NotificacionServicios()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Utilidades.Constantes.URL_BASE);
        }

        public async Task<ResultadoHttp> ObtenerNotificacionesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.GetAsync("notificaciones")
            );
        }

        public async Task<ResultadoHttp> EliminarNotificacion(string token, int notificacionId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.DeleteAsync($"notificaciones/{notificacionId}")
            );
        }

        public async Task<ResultadoHttp> EliminarNotificaciones(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await HttpHelper.EjecutarHttp(() =>
                _httpClient.DeleteAsync("notificaciones")
            );
        }
    }
}
