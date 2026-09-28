using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Grpc;

public sealed class CanalGrpc : IDisposable
{
    private readonly ISesionUsuario sesion;

    public CanalGrpc(IOptions<OpcionesServidor> opciones, ISesionUsuario sesion)
    {
        ArgumentNullException.ThrowIfNull(opciones);
        this.sesion = sesion;
        Canal = GrpcChannel.ForAddress(opciones.Value.UrlGrpc);
    }

    public GrpcChannel Canal { get; }

    public Metadata Encabezados() => new() { { "authorization", $"Bearer {sesion.Token}" } };

    public void Dispose() => Canal.Dispose();

    internal static ErrorOperacion TraducirError(RpcException excepcion) => excepcion.StatusCode switch
    {
        StatusCode.InvalidArgument => new ErrorOperacion(TipoError.Validacion, excepcion.Status.Detail),
        StatusCode.Unauthenticated => new ErrorOperacion(TipoError.NoAutenticado, excepcion.Status.Detail),
        StatusCode.PermissionDenied => new ErrorOperacion(TipoError.Prohibido, excepcion.Status.Detail),
        StatusCode.NotFound => new ErrorOperacion(TipoError.NoEncontrado, excepcion.Status.Detail),
        StatusCode.AlreadyExists => new ErrorOperacion(TipoError.Conflicto, excepcion.Status.Detail),
        StatusCode.Unavailable or StatusCode.DeadlineExceeded => new ErrorOperacion(TipoError.Conexion, Textos.ErrorConexion),
        _ => new ErrorOperacion(TipoError.Desconocido, Textos.ErrorServidor)
    };
}
