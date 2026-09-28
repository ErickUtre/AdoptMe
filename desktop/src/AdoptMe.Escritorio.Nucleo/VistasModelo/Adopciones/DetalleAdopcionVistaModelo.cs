using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;

public sealed partial class DetalleAdopcionVistaModelo(
    IServicioAdopciones adopciones,
    IServicioSolicitudes solicitudes,
    ISesionUsuario sesion,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeSolicitar))]
    private AdopcionCercana? adopcion;

    [ObservableProperty]
    private byte[]? foto;

    public bool PuedeSolicitar => Adopcion is not null && !sesion.EsAdministrador && Adopcion.PublicadorId != sesion.Perfil?.UsuarioID;

    public async Task AlNavegarAsync(object? parametro)
    {
        Adopcion = parametro as AdopcionCercana ?? throw new ArgumentException("Se esperaba una adopción.", nameof(parametro));
        var resultado = await adopciones.ObtenerFotoMascotaAsync(Adopcion.Mascota.MascotaID).ConfigureAwait(true);
        Foto = resultado.Exito ? resultado.Valor : null;
    }

    [RelayCommand]
    private async Task SolicitarAsync()
    {
        if (Adopcion is null)
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => solicitudes.RegistrarAsync(Adopcion.AdopcionId)).ConfigureAwait(true);
        if (!resultado.Exito && resultado.Error.Tipo == TipoError.Conflicto)
        {
            dialogos.Mostrar(Textos.SolicitudDuplicada, TipoMensaje.Advertencia, Textos.Advertencia);
            return;
        }
        Informar(dialogos, resultado, Textos.SolicitudEnviada);
    }

    [RelayCommand]
    private async Task VerVideoAsync()
    {
        if (Adopcion is null)
        {
            return;
        }
        var video = await MientrasOcupadoAsync(() => adopciones.ObtenerVideoMascotaAsync(Adopcion.Mascota.MascotaID)).ConfigureAwait(true);
        if (video.Exito)
        {
            dialogos.ReproducirVideo(video.Valor);
            return;
        }
        dialogos.Mostrar(Textos.SinVideo, TipoMensaje.Advertencia, Textos.Advertencia);
    }

    [RelayCommand]
    private void ExpandirFoto()
    {
        if (Foto is not null)
        {
            dialogos.MostrarImagen(Foto);
        }
    }

    [RelayCommand]
    private Task EnviarMensajeAsync() =>
        Adopcion is null ? Task.CompletedTask : navegador.NavegarAAsync<ChatVistaModelo>(Adopcion.PublicadorId);

    [RelayCommand]
    private Task RegresarAsync() => navegador.NavegarAAsync<MapaAdopcionesVistaModelo>();
}
