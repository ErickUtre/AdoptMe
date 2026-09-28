using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;

public sealed partial class EdicionAdopcionVistaModelo(
    IServicioAdopciones adopciones,
    IServicioArchivos archivos,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Mascota))]
    private Adopcion? adopcion;

    [ObservableProperty]
    private byte[]? foto;

    public Mascota Mascota => Adopcion?.Mascota ?? new Mascota();

    public async Task AlNavegarAsync(object? parametro)
    {
        Adopcion = parametro as Adopcion ?? throw new ArgumentException("Se esperaba una adopción.", nameof(parametro));
        await CargarFotoAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private Task EditarNombreAsync() => EditarAsync(CamposMascota.Nombre);

    [RelayCommand]
    private Task EditarEspecieAsync() => EditarAsync(CamposMascota.Especie);

    [RelayCommand]
    private Task EditarRazaAsync() => EditarAsync(CamposMascota.Raza);

    [RelayCommand]
    private Task EditarEdadAsync() => EditarAsync(CamposMascota.Edad);

    [RelayCommand]
    private Task EditarSexoAsync() => EditarAsync(CamposMascota.Sexo);

    [RelayCommand]
    private Task EditarTamanoAsync() => EditarAsync(CamposMascota.Tamano);

    [RelayCommand]
    private Task EditarDescripcionAsync() => EditarAsync(CamposMascota.Descripcion);

    [RelayCommand]
    private async Task CambiarFotoAsync()
    {
        if (Adopcion is null || dialogos.SeleccionarArchivo(FiltroArchivo.Imagenes) is not { } ruta)
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => archivos.SubirFotoMascotaAsync(Adopcion.MascotaID, ruta)).ConfigureAwait(true);
        if (Informar(dialogos, resultado))
        {
            await CargarFotoAsync().ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task CambiarVideoAsync()
    {
        if (Adopcion is null || dialogos.SeleccionarArchivo(FiltroArchivo.Videos) is not { } ruta)
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => archivos.SubirVideoMascotaAsync(Adopcion.MascotaID, ruta)).ConfigureAwait(true);
        Informar(dialogos, resultado, Textos.CampoActualizado);
    }

    [RelayCommand]
    private async Task VerVideoAsync()
    {
        if (Adopcion is null)
        {
            return;
        }
        var video = await MientrasOcupadoAsync(() => adopciones.ObtenerVideoMascotaAsync(Adopcion.MascotaID)).ConfigureAwait(true);
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
    private Task RegresarAsync() => navegador.NavegarAAsync<AdopcionesPropiasVistaModelo>();

    private async Task EditarAsync(Func<Mascota, CampoEditable> crearCampo)
    {
        if (Adopcion?.Mascota is null)
        {
            return;
        }
        var campo = crearCampo(Adopcion.Mascota);
        if (dialogos.Abrir<EditarCampoVistaModelo, string>(campo) is not { } nuevoValor)
        {
            return;
        }
        var cambios = new Dictionary<string, string> { [campo.Clave] = nuevoValor };
        var resultado = await MientrasOcupadoAsync(() => adopciones.ModificarMascotaAsync(Adopcion.AdopcionID, cambios)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, out var actualizada, Textos.CampoActualizado))
        {
            Adopcion = actualizada;
        }
    }

    private async Task CargarFotoAsync()
    {
        if (Adopcion is null)
        {
            return;
        }
        var resultado = await adopciones.ObtenerFotoMascotaAsync(Adopcion.MascotaID).ConfigureAwait(true);
        Foto = resultado.Exito ? resultado.Valor : null;
    }
}
