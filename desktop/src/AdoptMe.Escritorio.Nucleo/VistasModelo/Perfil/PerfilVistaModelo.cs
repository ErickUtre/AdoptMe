using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.Validacion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Perfil;

public sealed partial class PerfilVistaModelo(
    IServicioCuentas cuentas,
    IServicioArchivos archivos,
    ISesionUsuario sesion,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescripcionUbicacion))]
    private Modelos.Perfil? perfil;

    [ObservableProperty]
    private byte[]? foto;

    public string DescripcionUbicacion => Perfil?.Ubicacion is { } ubicacion ? SeleccionUbicacionVistaModelo.Describir(ubicacion) : string.Empty;

    public Task AlNavegarAsync(object? parametro)
    {
        Perfil = sesion.Perfil;
        Foto = sesion.FotoPerfil;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EditarNombreAsync()
    {
        var campo = new CampoEditable("Nombre", "Nombre", TipoCampo.Texto, Perfil?.Nombre ?? string.Empty, 100,
            Validar: valor => Validador.EsNombreValido(valor) ? null : Textos.NombreInvalido);
        if (dialogos.Abrir<EditarCampoVistaModelo, string>(campo) is { } nombre)
        {
            await ActualizarPerfilAsync(Validador.Normalizar(nombre), null).ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task EditarTelefonoAsync()
    {
        var campo = new CampoEditable("Telefono", "Teléfono", TipoCampo.Texto, Perfil?.Telefono ?? string.Empty, 10,
            Validar: valor => Validador.EsTelefonoValido(valor) ? null : Textos.TelefonoInvalido);
        if (dialogos.Abrir<EditarCampoVistaModelo, string>(campo) is { } telefono)
        {
            await ActualizarPerfilAsync(null, telefono).ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task EditarCorreoAsync()
    {
        if (Perfil is null)
        {
            return;
        }
        var campo = new CampoEditable("Correo", "Correo", TipoCampo.Texto, Perfil.Acceso.Correo, 100,
            Validar: valor => Validador.EsCorreoValido(valor) ? null : Textos.CorreoInvalido);
        if (dialogos.Abrir<EditarCampoVistaModelo, string>(campo) is not { } correo)
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => cuentas.ActualizarCorreoAsync(correo)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, Textos.PerfilActualizado))
        {
            Publicar(Perfil with { Acceso = Perfil.Acceso with { Correo = correo.Trim().ToLowerInvariant() } });
        }
    }

    [RelayCommand]
    private async Task ActualizarUbicacionAsync()
    {
        if (Perfil is null || dialogos.Abrir<SeleccionUbicacionVistaModelo, Ubicacion>(Perfil.Ubicacion) is not { } ubicacion)
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => cuentas.ActualizarUbicacionAsync(ubicacion)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, out var nuevaUbicacion, Textos.PerfilActualizado))
        {
            Publicar(Perfil with { Ubicacion = nuevaUbicacion });
        }
    }

    [RelayCommand]
    private async Task CambiarFotoAsync()
    {
        if (dialogos.SeleccionarArchivo(FiltroArchivo.Imagenes) is not { } ruta)
        {
            return;
        }
        var subida = await MientrasOcupadoAsync(() => archivos.SubirFotoPerfilAsync(ruta)).ConfigureAwait(true);
        if (!Informar(dialogos, subida))
        {
            return;
        }
        var nueva = await cuentas.ObtenerFotoPerfilAsync().ConfigureAwait(true);
        if (nueva.Exito)
        {
            Foto = nueva.Valor;
            sesion.ActualizarFotoPerfil(nueva.Valor);
        }
    }

    [RelayCommand]
    private void ExpandirFoto()
    {
        if (Foto is not null)
        {
            dialogos.MostrarImagen(Foto);
        }
    }

    private async Task ActualizarPerfilAsync(string? nombre, string? telefono)
    {
        var resultado = await MientrasOcupadoAsync(() => cuentas.ActualizarPerfilAsync(nombre, telefono)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, out var actualizado, Textos.PerfilActualizado))
        {
            Publicar(actualizado);
        }
    }

    private void Publicar(Modelos.Perfil actualizado)
    {
        Perfil = actualizado;
        sesion.ActualizarPerfil(actualizado);
    }
}
