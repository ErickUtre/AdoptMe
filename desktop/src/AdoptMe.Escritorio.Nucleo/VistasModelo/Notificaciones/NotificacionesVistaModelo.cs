using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Notificaciones;

public sealed partial class NotificacionesVistaModelo(
    IServicioNotificaciones notificaciones,
    ICanalNotificaciones canal,
    INavegador navegador,
    IDespachadorUi despachador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar, IAlSalir
{
    public ObservableCollection<Notificacion> Elementos { get; } = [];

    public async Task AlNavegarAsync(object? parametro)
    {
        canal.NotificacionRecibida += AlRecibir;
        await CargarAsync().ConfigureAwait(true);
    }

    public void AlSalir() => canal.NotificacionRecibida -= AlRecibir;

    [RelayCommand]
    private Task AbrirAsync(Notificacion notificacion) => notificacion switch
    {
        { Tipo: TiposNotificacion.AdopcionCercana, ReferenciaID: int adopcionId } => navegador.NavegarAAsync<MapaAdopcionesVistaModelo>(adopcionId),
        { Tipo: TiposNotificacion.SolicitudRecibida } => navegador.NavegarAAsync<AdopcionesPropiasVistaModelo>(),
        _ => Task.CompletedTask
    };

    [RelayCommand]
    private async Task EliminarAsync(Notificacion notificacion)
    {
        var resultado = await MientrasOcupadoAsync(() => notificaciones.EliminarAsync(notificacion.NotificacionID)).ConfigureAwait(true);
        if (Informar(dialogos, resultado))
        {
            Elementos.Remove(notificacion);
        }
    }

    [RelayCommand]
    private async Task EliminarTodasAsync()
    {
        var resultado = await MientrasOcupadoAsync(notificaciones.EliminarTodasAsync).ConfigureAwait(true);
        if (Informar(dialogos, resultado))
        {
            Elementos.Clear();
        }
    }

    private async Task CargarAsync()
    {
        var resultado = await MientrasOcupadoAsync(notificaciones.ListarAsync).ConfigureAwait(true);
        if (!Informar(dialogos, resultado, out var lista))
        {
            return;
        }
        Elementos.Clear();
        foreach (var notificacion in lista)
        {
            Elementos.Add(notificacion);
        }
    }

    private void AlRecibir(object? remitente, Notificacion notificacion) =>
        despachador.Ejecutar(() => Elementos.Insert(0, notificacion));
}
