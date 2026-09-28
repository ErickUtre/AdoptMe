using System.Globalization;
using AdoptMe.Escritorio.Grpc.Notificacion;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NotificacionModelo = AdoptMe.Escritorio.Nucleo.Modelos.Notificacion;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Grpc;

public interface ICanalNotificaciones
{
    event EventHandler<NotificacionModelo>? NotificacionRecibida;

    void Iniciar();

    void Detener();
}

public sealed partial class CanalNotificaciones(CanalGrpc canal, ILogger<CanalNotificaciones> registro) : ICanalNotificaciones, IDisposable
{
    private static readonly TimeSpan EsperaReconexion = TimeSpan.FromSeconds(5);

    private readonly ServicioNotificacion.ServicioNotificacionClient cliente = new(canal.Canal);
    private CancellationTokenSource? cancelacion;

    public event EventHandler<NotificacionModelo>? NotificacionRecibida;

    public void Iniciar()
    {
        Detener();
        cancelacion = new CancellationTokenSource();
        _ = EscucharAsync(cancelacion.Token);
    }

    public void Detener()
    {
        cancelacion?.Cancel();
        cancelacion?.Dispose();
        cancelacion = null;
    }

    public void Dispose() => Detener();

    private async Task EscucharAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var llamada = cliente.EscucharNotificaciones(new SolicitudSuscripcion(), canal.Encabezados(), cancellationToken: token);
                await foreach (var notificacion in llamada.ResponseStream.ReadAllAsync(token).ConfigureAwait(false))
                {
                    NotificacionRecibida?.Invoke(this, Convertir(notificacion));
                }
            }
            catch (RpcException excepcion) when (excepcion.StatusCode == StatusCode.Cancelled || token.IsCancellationRequested)
            {
                return;
            }
            catch (RpcException excepcion) when (excepcion.StatusCode == StatusCode.Unauthenticated)
            {
                RegistrarSesionRechazada(registro, excepcion.Status.Detail);
                return;
            }
            catch (RpcException excepcion)
            {
                RegistrarReconexion(registro, excepcion.Status.Detail);
            }

            await EsperarAsync(token).ConfigureAwait(false);
        }
    }

    private static async Task EsperarAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(EsperaReconexion, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    private static NotificacionModelo Convertir(Notificacion origen) => new()
    {
        NotificacionID = origen.NotificacionId,
        Titulo = origen.Titulo,
        Mensaje = origen.Mensaje,
        Tipo = origen.Tipo,
        ReferenciaID = origen.ReferenciaId == 0 ? null : origen.ReferenciaId,
        ReferenciaTipo = string.IsNullOrEmpty(origen.ReferenciaTipo) ? null : origen.ReferenciaTipo,
        FechaCreacion = DateTime.TryParse(origen.Fecha, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha)
            ? fecha.ToLocalTime()
            : DateTime.Now
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "El servidor rechazó la suscripción a notificaciones: {Detalle}")]
    private static partial void RegistrarSesionRechazada(ILogger registro, string detalle);

    [LoggerMessage(Level = LogLevel.Information, Message = "Se perdió la conexión de notificaciones; reintentando: {Detalle}")]
    private static partial void RegistrarReconexion(ILogger registro, string detalle);
}
