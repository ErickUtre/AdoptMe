using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.Servicios.TiempoReal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;

public sealed record MensajeVisible(int ChatId, string Contenido, DateTime FechaEnvio, bool EsPropio)
{
    public string Hora => FechaEnvio.ToLocalTime().ToString("HH:mm", System.Globalization.CultureInfo.CurrentCulture);
}

public sealed partial class ChatVistaModelo(
    IServicioChat chat,
    IServicioCuentas cuentas,
    ICanalChat canal,
    ISesionUsuario sesion,
    IDespachadorUi despachador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar, IAlSalir
{
    public const int LongitudMaxima = 1000;

    private int contraparteId;

    [ObservableProperty]
    private string titulo = Textos.UsuarioDesconocido;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarCommand))]
    private string textoMensaje = string.Empty;

    public ObservableCollection<MensajeVisible> Mensajes { get; } = [];

    public async Task AlNavegarAsync(object? parametro)
    {
        contraparteId = parametro as int? ?? throw new ArgumentException("Se esperaba el identificador del usuario.", nameof(parametro));
        canal.MensajeRecibido += AlRecibirMensaje;
        canal.MensajeRechazado += AlRechazarMensaje;

        var resumen = await cuentas.ObtenerResumenAsync(contraparteId).ConfigureAwait(true);
        Titulo = resumen.Exito ? resumen.Valor.Nombre : Textos.UsuarioDesconocido;

        var historial = await MientrasOcupadoAsync(() => chat.ListarMensajesAsync(contraparteId)).ConfigureAwait(true);
        if (!Informar(dialogos, historial, out var mensajes))
        {
            return;
        }
        Mensajes.Clear();
        foreach (var mensaje in mensajes)
        {
            Agregar(mensaje);
        }
        await canal.ConectarAsync().ConfigureAwait(true);
    }

    public void AlSalir()
    {
        canal.MensajeRecibido -= AlRecibirMensaje;
        canal.MensajeRechazado -= AlRechazarMensaje;
    }

    private bool PuedeEnviar() => !string.IsNullOrWhiteSpace(TextoMensaje) && TextoMensaje.Length <= LongitudMaxima;

    [RelayCommand(CanExecute = nameof(PuedeEnviar))]
    private async Task EnviarAsync()
    {
        var contenido = TextoMensaje.Trim();
        TextoMensaje = string.Empty;
        await canal.EnviarAsync(contraparteId, contenido).ConfigureAwait(true);
    }

    internal bool PerteneceALaConversacion(Mensaje mensaje) =>
        (mensaje.RemitenteID == contraparteId && mensaje.DestinatarioID == sesion.UsuarioId)
        || (mensaje.RemitenteID == sesion.UsuarioId && mensaje.DestinatarioID == contraparteId);

    private void AlRecibirMensaje(object? remitente, Mensaje mensaje)
    {
        if (PerteneceALaConversacion(mensaje))
        {
            despachador.Ejecutar(() => Agregar(mensaje));
        }
    }

    private void AlRechazarMensaje(object? remitente, string motivo) =>
        despachador.Ejecutar(() => dialogos.Mostrar(motivo, TipoMensaje.Advertencia, Textos.Advertencia));

    private void Agregar(Mensaje mensaje)
    {
        if (Mensajes.Any(existente => existente.ChatId == mensaje.ChatID))
        {
            return;
        }
        Mensajes.Add(new MensajeVisible(mensaje.ChatID, mensaje.Contenido, mensaje.FechaEnvio, mensaje.RemitenteID == sesion.UsuarioId));
    }
}
