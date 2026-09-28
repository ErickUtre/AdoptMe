using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Solicitudes;

public sealed record DesenlaceSolicitudes(bool AdopcionAceptada, int? ContactarUsuarioId);

public sealed partial class SolicitudesVistaModelo(IServicioSolicitudes solicitudes, IDialogos dialogos)
    : VistaModeloDialogo<DesenlaceSolicitudes>, IAlNavegar
{
    private int adopcionId;
    private bool adopcionAceptada;

    [ObservableProperty]
    private bool sinSolicitudes;

    public ObservableCollection<Solicitud> Pendientes { get; } = [];

    public override void Inicializar(object? parametro) =>
        adopcionId = parametro as int? ?? throw new ArgumentException("Se esperaba el identificador de la adopción.", nameof(parametro));

    public async Task AlNavegarAsync(object? parametro)
    {
        var resultado = await MientrasOcupadoAsync(() => solicitudes.ListarAsync(adopcionId)).ConfigureAwait(true);
        if (!Informar(dialogos, resultado, out var lista))
        {
            return;
        }
        Pendientes.Clear();
        foreach (var solicitud in lista)
        {
            Pendientes.Add(solicitud);
        }
        SinSolicitudes = Pendientes.Count == 0;
    }

    [RelayCommand]
    private async Task AceptarSolicitudAsync(Solicitud solicitud)
    {
        if (!dialogos.Confirmar(Textos.ConfirmarAceptacion(solicitud.NombreAdoptante), Textos.Confirmacion))
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => solicitudes.AceptarAsync(adopcionId, solicitud.SolicitudID)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, Textos.SolicitudAceptada))
        {
            adopcionAceptada = true;
            Aceptar(new DesenlaceSolicitudes(true, null));
        }
    }

    [RelayCommand]
    private async Task RechazarSolicitudAsync(Solicitud solicitud)
    {
        if (!dialogos.Confirmar(Textos.ConfirmarRechazo(solicitud.NombreAdoptante), Textos.Confirmacion))
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => solicitudes.RechazarAsync(adopcionId, solicitud.SolicitudID)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, Textos.SolicitudRechazada))
        {
            Pendientes.Remove(solicitud);
            SinSolicitudes = Pendientes.Count == 0;
        }
    }

    [RelayCommand]
    private void Contactar(Solicitud solicitud) => Aceptar(new DesenlaceSolicitudes(adopcionAceptada, solicitud.AdoptanteID));

    [RelayCommand]
    private void Cerrar() => Aceptar(new DesenlaceSolicitudes(adopcionAceptada, null));
}
