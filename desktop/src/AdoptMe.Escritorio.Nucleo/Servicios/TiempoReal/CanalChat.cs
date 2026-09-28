using AdoptMe.Escritorio.Nucleo.Configuracion;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using Microsoft.Extensions.Options;
using SocketIOClient;
using SocketIOClient.Transport;

namespace AdoptMe.Escritorio.Nucleo.Servicios.TiempoReal;

public interface ICanalChat
{
    event EventHandler<Mensaje>? MensajeRecibido;

    event EventHandler<string>? MensajeRechazado;

    Task ConectarAsync();

    Task DesconectarAsync();

    Task EnviarAsync(int destinatarioId, string contenido);
}

public sealed class CanalChat(IOptions<OpcionesServidor> opciones, ISesionUsuario sesion) : ICanalChat, IAsyncDisposable
{
    private const string EventoEnviar = "enviar_mensaje";
    private const string EventoNuevo = "nuevo_mensaje";
    private const string EventoError = "error_mensaje";

    private readonly SemaphoreSlim candado = new(1, 1);
    private SocketIOClient.SocketIO? cliente;

    public event EventHandler<Mensaje>? MensajeRecibido;

    public event EventHandler<string>? MensajeRechazado;

    public Task ConectarAsync() => ObtenerClienteConectadoAsync();

    private async Task<SocketIOClient.SocketIO> ObtenerClienteConectadoAsync()
    {
        await candado.WaitAsync().ConfigureAwait(false);
        try
        {
            if (cliente is { Connected: true })
            {
                return cliente;
            }
            cliente?.Dispose();
            cliente = CrearCliente();
            await cliente.ConnectAsync().ConfigureAwait(false);
            return cliente;
        }
        finally
        {
            candado.Release();
        }
    }

    public async Task DesconectarAsync()
    {
        await candado.WaitAsync().ConfigureAwait(false);
        try
        {
            if (cliente is null)
            {
                return;
            }
            await cliente.DisconnectAsync().ConfigureAwait(false);
            cliente.Dispose();
            cliente = null;
        }
        finally
        {
            candado.Release();
        }
    }

    public async Task EnviarAsync(int destinatarioId, string contenido)
    {
        var conectado = await ObtenerClienteConectadoAsync().ConfigureAwait(false);
        await conectado.EmitAsync(EventoEnviar, new { DestinatarioID = destinatarioId, Contenido = contenido }).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DesconectarAsync().ConfigureAwait(false);
        candado.Dispose();
    }

    private SocketIOClient.SocketIO CrearCliente()
    {
        var nuevo = new SocketIOClient.SocketIO(opciones.Value.UrlTiempoReal, new SocketIOOptions
        {
            Auth = new { token = sesion.Token },
            Transport = TransportProtocol.WebSocket,
            Reconnection = true
        });
        nuevo.On(EventoNuevo, respuesta => MensajeRecibido?.Invoke(this, respuesta.GetValue<Mensaje>()));
        nuevo.On(EventoError, respuesta => MensajeRechazado?.Invoke(this, respuesta.GetValue<ErrorMensaje>().Mensaje));
        return nuevo;
    }

    private sealed record ErrorMensaje(string Mensaje);
}
