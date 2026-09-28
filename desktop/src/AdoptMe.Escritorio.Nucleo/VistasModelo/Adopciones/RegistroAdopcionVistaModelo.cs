using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;

public sealed partial class RegistroAdopcionVistaModelo(
    IServicioAdopciones adopciones,
    IServicioArchivos archivos,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase
{
    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string especie = string.Empty;

    [ObservableProperty]
    private string raza = string.Empty;

    [ObservableProperty]
    private int? anios;

    [ObservableProperty]
    private int? meses;

    [ObservableProperty]
    private string? sexo;

    [ObservableProperty]
    private string? tamano;

    [ObservableProperty]
    private string descripcion = string.Empty;

    [ObservableProperty]
    private string? rutaFoto;

    [ObservableProperty]
    private string? rutaVideo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescripcionUbicacion))]
    private Ubicacion? ubicacion;

    public string DescripcionUbicacion => Ubicacion is null ? string.Empty : SeleccionUbicacionVistaModelo.Describir(Ubicacion);

    [RelayCommand]
    private void SeleccionarFoto() => RutaFoto = dialogos.SeleccionarArchivo(FiltroArchivo.Imagenes) ?? RutaFoto;

    [RelayCommand]
    private void SeleccionarVideo() => RutaVideo = dialogos.SeleccionarArchivo(FiltroArchivo.Videos) ?? RutaVideo;

    [RelayCommand]
    private void SeleccionarUbicacion()
    {
        if (Ubicacion is not null && !dialogos.Confirmar(Textos.ConfirmarModificarUbicacion, Textos.Confirmacion))
        {
            return;
        }
        Ubicacion = dialogos.Abrir<SeleccionUbicacionVistaModelo, Ubicacion>(Ubicacion) ?? Ubicacion;
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (ConstruirMascota() is not { } mascota || Ubicacion is null || RutaFoto is null)
        {
            dialogos.Mostrar(RutaFoto is null ? Textos.FotoObligatoria : Textos.CamposObligatorios, TipoMensaje.Advertencia, Textos.Advertencia);
            return;
        }

        var registro = await MientrasOcupadoAsync(() => adopciones.RegistrarAsync(mascota, Ubicacion)).ConfigureAwait(true);
        if (!Informar(dialogos, registro, out var registrada))
        {
            return;
        }

        var errores = await MientrasOcupadoAsync(() => SubirArchivosAsync(registrada.MascotaID)).ConfigureAwait(true);
        foreach (var error in errores)
        {
            dialogos.Mostrar(Textos.ArchivoNoSubido(error), TipoMensaje.Advertencia, Textos.Advertencia);
        }
        dialogos.Mostrar(Textos.AdopcionRegistrada, TipoMensaje.Informacion, Textos.Exito);
        await navegador.NavegarAAsync<MapaAdopcionesVistaModelo>().ConfigureAwait(true);
    }

    private async Task<IReadOnlyList<string>> SubirArchivosAsync(int mascotaId)
    {
        var errores = new List<string>();
        if (RutaFoto is not null && await archivos.SubirFotoMascotaAsync(mascotaId, RutaFoto).ConfigureAwait(true) is { Exito: false } foto)
        {
            errores.Add(foto.Error.Mensaje);
        }
        if (RutaVideo is not null && await archivos.SubirVideoMascotaAsync(mascotaId, RutaVideo).ConfigureAwait(true) is { Exito: false } video)
        {
            errores.Add(video.Error.Mensaje);
        }
        return errores;
    }

    internal Mascota? ConstruirMascota()
    {
        var textos = new[] { Nombre, Especie, Raza, Descripcion };
        if (textos.Any(string.IsNullOrWhiteSpace) || Anios is null || Meses is null || Sexo is null || Tamano is null)
        {
            return null;
        }
        return new Mascota
        {
            Nombre = Nombre.Trim(),
            Especie = Especie.Trim(),
            Raza = Raza.Trim(),
            Edad = CamposMascota.DescribirEdad(Anios.Value, Meses.Value),
            Sexo = Sexo,
            Tamano = Tamano,
            Descripcion = Descripcion.Trim()
        };
    }
}
