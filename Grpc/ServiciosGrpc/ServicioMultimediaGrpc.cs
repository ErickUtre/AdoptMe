using Cliente_AdoptMe.Utilidades;
using Grpc.Core;
using MultimediaGrpc;
using GrpcMetadata = Grpc.Core.Metadata;
using ProtoMetadata = MultimediaGrpc.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_AdoptMe.Grpc.ServiciosGrpc
{
    public class ServicioMultimediaGrpc
    {
        private readonly ServicioMultimedia.ServicioMultimediaClient _cliente;
        private readonly Channel _canal;

        public ServicioMultimediaGrpc()
        {
            _canal = new Channel(Constantes.URL_GRPC, ChannelCredentials.Insecure);
            _cliente = new ServicioMultimedia.ServicioMultimediaClient(_canal);
        }

        public ServicioMultimedia.ServicioMultimediaClient Cliente => _cliente;

        public async Task SubirArchivoAsync(
            string rutaArchivo,
            int idReferencia,
            string tokenJwt,
            Func<GrpcMetadata, AsyncClientStreamingCall<ChunkArchivo, RespuestaGeneral>> metodoGrpc,
            string[] extensionesPermitidas)
        {
            var metadata = new GrpcMetadata
            {
                { "authorization", $"Bearer {tokenJwt}" }
            };

            var call = metodoGrpc(metadata);

            string nombreArchivo = Path.GetFileName(rutaArchivo);
            string extension = Path.GetExtension(nombreArchivo).ToLower();

            if (!extensionesPermitidas.Contains(extension))
            {
                MessageBox.Show($"Formato no permitido: {extension}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await call.RequestStream.WriteAsync(new ChunkArchivo
            {
                Metadata = new ProtoMetadata
                {
                    IdReferencia = idReferencia,
                    NombreArchivo = nombreArchivo
                }
            });

            await Task.Delay(200);

            const int chunkSize = 64 * 1024;
            byte[] buffer = new byte[chunkSize];

            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                int bytesRead;
                while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await call.RequestStream.WriteAsync(new ChunkArchivo
                    {
                        Chunk = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead)
                    });
                }
            }

            await call.RequestStream.CompleteAsync();
            var respuesta = await call.ResponseAsync;

            MessageBox.Show(respuesta.Mensaje, "Resultado",
                MessageBoxButton.OK,
                respuesta.Exito ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

    }
}
