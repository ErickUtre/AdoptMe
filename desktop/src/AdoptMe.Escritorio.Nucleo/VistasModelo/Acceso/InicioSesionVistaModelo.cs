using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;

public sealed partial class InicioSesionVistaModelo(
    IServicioCuentas cuentas,
    ISesionUsuario sesion,
    IDialogos dialogos,
    IVentanas ventanas) : VistaModeloBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IniciarSesionCommand))]
    private string correo = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IniciarSesionCommand))]
    private string contrasena = string.Empty;

    [ObservableProperty]
    private bool contrasenaVisible;

    private bool PuedeIniciarSesion() => !string.IsNullOrWhiteSpace(Correo) && !string.IsNullOrEmpty(Contrasena) && !EstaOcupado;

    [RelayCommand(CanExecute = nameof(PuedeIniciarSesion))]
    private async Task IniciarSesionAsync()
    {
        var resultado = await MientrasOcupadoAsync(() => cuentas.IniciarSesionAsync(Correo.Trim(), Contrasena));
        if (!resultado.Exito)
        {
            var mensaje = resultado.Error.Tipo == TipoError.NoAutenticado ? Textos.CredencialesIncorrectas : resultado.Error.Mensaje;
            dialogos.Mostrar(mensaje, TipoMensaje.Error, Textos.Error);
            return;
        }
        sesion.Iniciar(resultado.Valor);
        Contrasena = string.Empty;
        ventanas.MostrarPrincipal();
    }

    [RelayCommand]
    private void IrARegistro() => dialogos.Abrir<RegistroUsuarioVistaModelo, bool>();

    [RelayCommand]
    private void AlternarVisibilidadContrasena() => ContrasenaVisible = !ContrasenaVisible;
}
