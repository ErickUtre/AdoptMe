using Cliente_AdoptMe.Utilidades;
using Grpc.Core;
using GrpcMetadata = Grpc.Core.Metadata;
using NotificacionGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Grpc.ServiciosGrpc
{
    public class ServicioNotificacionGrpc
    {
        private readonly ServicioNotificacion.ServicioNotificacionClient _cliente;
        private readonly Channel _canal;

        public event Action<Notificacion> NotificacionRecibida;

        public ServicioNotificacionGrpc()
        {
            _canal = new Channel(Constantes.URL_GRPC, ChannelCredentials.Insecure);
            _cliente = new ServicioNotificacion.ServicioNotificacionClient(_canal);
        }

        public async Task EscucharNotificacionesAsync(string jwt, CancellationToken cancellationToken)
        {
            var headers = new GrpcMetadata
            {
                { "authorization", $"Bearer {jwt}" }
            };

            var call = _cliente.EscucharNotificaciones(new Empty(), headers, cancellationToken: cancellationToken);

            try
            {
                while (await call.ResponseStream.MoveNext(cancellationToken))
                {
                    var notificacion = call.ResponseStream.Current;
                    NotificacionRecibida?.Invoke(notificacion);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelado por el cliente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en stream de notificaciones: {ex.Message}");
            }
            finally
            {
                if (call is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public void Cerrar()
        {
            _canal.ShutdownAsync().Wait();
        }
    }
}
