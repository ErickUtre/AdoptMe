using Cliente_AdoptMe.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public static class HttpHelper
    {
        public static async Task<ResultadoHttp> EjecutarHttp(Func<Task<HttpResponseMessage>> accionHttp)
        {
            try
            {
                var respuesta = await accionHttp();

                if (respuesta.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return new ResultadoHttp
                    {
                        Exito = false,
                        Respuesta = respuesta,
                        MensajeError = Properties.Resources.mensaje_ErrorBD,
                        Codigo = respuesta.StatusCode
                    };
                }

                if (respuesta.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ResultadoHttp
                    {
                        Exito = false,
                        Respuesta = respuesta,
                        MensajeError = Properties.Resources.mensaje_NoAutenticado,
                        Codigo = respuesta.StatusCode
                    };
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    Console.WriteLine(respuesta);
                    return new ResultadoHttp
                    {
                        Exito = false,
                        Respuesta = respuesta,
                        MensajeError = Properties.Resources.mensaje_ErrorServidor,
                        Codigo = respuesta.StatusCode
                    };
                }

                return new ResultadoHttp
                {
                    Exito = true,
                    Respuesta = respuesta,
                    Codigo = respuesta.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                return new ResultadoHttp
                {
                    Exito = false,
                    MensajeError = Properties.Resources.mensaje_ErrorServidor,
                };
            }
            catch (TaskCanceledException ex)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                return new ResultadoHttp
                {
                    Exito = false,
                    MensajeError = Properties.Resources.global_ErrorTiempo
                };
            }
            catch (Exception ex)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                return new ResultadoHttp
                {
                    Exito = false,
                    MensajeError = Properties.Resources.mensaje_ErrorGeneral
                };
            }
        }
    }
}
