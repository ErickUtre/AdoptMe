using ChatGrpc;
using Cliente_AdoptMe.Utilidades;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Grpc
{
    public class ServicioMensajeGrpc
    {
        private readonly ServicioChat.ServicioChatClient _cliente;
        private readonly Channel _canal;

        public ServicioMensajeGrpc()
        {
            _canal = new Channel(Constantes.URL_GRPC, ChannelCredentials.Insecure);
            _cliente = new ServicioChat.ServicioChatClient(_canal);
        }

        public async Task PublicarMensajeAsync(int remitenteId, int receptorId, string contenido)
        {
            var mensaje = new MensajeChat
            {
                RemitenteID = remitenteId,
                ReceptorID = receptorId,
                Contenido = contenido
            };

            await _cliente.PublicarMensajeAsync(mensaje);
        }

        public async Task SuscribirseMensajesAsync(Action<MensajeChat> onMensajeRecibido, CancellationToken cancellationToken)
        {
            var respuesta = _cliente.SuscribirMensajes(new Empty());

            try
            {
                while (await respuesta.ResponseStream.MoveNext(cancellationToken))
                {
                    var mensaje = respuesta.ResponseStream.Current;
                    onMensajeRecibido?.Invoke(mensaje);
                }
            }
            finally
            {
                respuesta?.Dispose();
            }
        }

        public async Task<List<Usuario>> ObtenerContactosAsync(int usuarioId)
        {
            var solicitud = new SolicitudObtenerContactos
            {
                UsuarioID = usuarioId
            };

            var respuesta = await _cliente.ObtenerContactosAsync(solicitud);

            return respuesta.Contactos
                .Select(c => new Usuario
                {
                    UsuarioID = c.UsuarioID,
                    Nombre = c.Nombre,
                })
                .ToList();
        }

        public async Task CerrarConexionAsync()
        {
            await _canal.ShutdownAsync();
        }
    }
}
