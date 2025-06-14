using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Utilidades;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UbicacionGrpc;

namespace Cliente_AdoptMe.Grpc
{
    public class ServicioUbicacionGrpc
    {
        private readonly ServicioUbicacion.ServicioUbicacionClient _cliente;

        public ServicioUbicacionGrpc()
        {
            var channel = new Channel(Constantes.URL_GRPC, ChannelCredentials.Insecure);
            _cliente = new ServicioUbicacion.ServicioUbicacionClient(channel);
        }

        public async Task<List<SolicitudCercana>> ObtenerSolicitudesCercanasAsync(double lat, double lon)
        {
            var resultadoLista = new List<SolicitudCercana>();

            var request = new UbicacionGrpc.Ubicacion
            {
                Latitud = lat,
                Longitud = lon
            };

            var headers = new Metadata
            {
                { "authorization", $"Bearer {UsuarioSingleton.Instancia.Token}" }
            };

            try
            {
                var respuesta = await _cliente.ObtenerSolicitudesCercanasAsync(request, headers);
                if (respuesta?.Resultados != null && respuesta.Resultados.Count > 0)
                {
                    foreach (var s in respuesta.Resultados)
                    {
                        Console.WriteLine($"ID: {s.SolicitudAdopcionId}, Distancia: {s.Distancia}, Coord: ({s.Latitud}, {s.Longitud})");
                        resultadoLista.Add(s);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron solicitudes cercanas.");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error gRPC: {ex.Status.Detail}");
            }

            return resultadoLista;
        }
    }
}
