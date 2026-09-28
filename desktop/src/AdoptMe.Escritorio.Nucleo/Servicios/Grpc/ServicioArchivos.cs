using AdoptMe.Escritorio.Grpc.Multimedia;
using AdoptMe.Escritorio.Nucleo.Comun;
using Google.Protobuf;
using Grpc.Core;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Grpc;

public interface IServicioArchivos
{
    Task<Resultado> SubirFotoPerfilAsync(string rutaArchivo);

    Task<Resultado> SubirFotoMascotaAsync(int mascotaId, string rutaArchivo);

    Task<Resultado> SubirVideoMascotaAsync(int mascotaId, string rutaArchivo);
}

public sealed class ServicioArchivos(CanalGrpc canal) : IServicioArchivos
{
    private const int TamanoFragmento = 64 * 1024;

    private readonly ServicioMultimedia.ServicioMultimediaClient cliente = new(canal.Canal);

    public Task<Resultado> SubirFotoPerfilAsync(string rutaArchivo) =>
        SubirAsync(encabezados => cliente.SubirFotoUsuario(encabezados), 0, rutaArchivo);

    public Task<Resultado> SubirFotoMascotaAsync(int mascotaId, string rutaArchivo) =>
        SubirAsync(encabezados => cliente.SubirFotoMascota(encabezados), mascotaId, rutaArchivo);

    public Task<Resultado> SubirVideoMascotaAsync(int mascotaId, string rutaArchivo) =>
        SubirAsync(encabezados => cliente.SubirVideoMascota(encabezados), mascotaId, rutaArchivo);

    private async Task<Resultado> SubirAsync(
        Func<Metadata, AsyncClientStreamingCall<FragmentoArchivo, ResultadoSubida>> iniciarLlamada,
        int idReferencia,
        string rutaArchivo)
    {
        try
        {
            using var llamada = iniciarLlamada(canal.Encabezados());
            await llamada.RequestStream.WriteAsync(new FragmentoArchivo
            {
                Metadatos = new MetadatosArchivo { IdReferencia = idReferencia, NombreArchivo = Path.GetFileName(rutaArchivo) }
            }).ConfigureAwait(false);

            await using (var archivo = File.OpenRead(rutaArchivo))
            {
                var bufer = new byte[TamanoFragmento];
                int leidos;
                while ((leidos = await archivo.ReadAsync(bufer).ConfigureAwait(false)) > 0)
                {
                    await llamada.RequestStream.WriteAsync(new FragmentoArchivo { Datos = ByteString.CopyFrom(bufer, 0, leidos) }).ConfigureAwait(false);
                }
            }

            await llamada.RequestStream.CompleteAsync().ConfigureAwait(false);
            var respuesta = await llamada.ResponseAsync.ConfigureAwait(false);
            return respuesta.Exito
                ? Resultado.Correcto()
                : Resultado.Fallo(new ErrorOperacion(TipoError.Desconocido, respuesta.Mensaje));
        }
        catch (RpcException excepcion)
        {
            return Resultado.Fallo(CanalGrpc.TraducirError(excepcion));
        }
        catch (IOException excepcion)
        {
            return Resultado.Fallo(new ErrorOperacion(TipoError.Validacion, excepcion.Message));
        }
    }
}
