using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Validacion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;

public sealed partial class RegistroUsuarioVistaModelo(IServicioCuentas cuentas, IDialogos dialogos) : VistaModeloDialogo<bool>
{
    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    [ObservableProperty]
    private string contrasena = string.Empty;

    [ObservableProperty]
    private string confirmacionContrasena = string.Empty;

    [ObservableProperty]
    private bool contrasenaVisible;

    [ObservableProperty]
    private bool nombreInvalido;

    [ObservableProperty]
    private bool correoInvalido;

    [ObservableProperty]
    private bool telefonoInvalido;

    [ObservableProperty]
    private bool contrasenaInvalida;

    [ObservableProperty]
    private bool confirmacionInvalida;

    [ObservableProperty]
    private Ubicacion? ubicacion;

    [RelayCommand]
    private void AlternarVisibilidadContrasena() => ContrasenaVisible = !ContrasenaVisible;

    [RelayCommand]
    private void SeleccionarUbicacion()
    {
        var mensaje = Ubicacion is null ? Textos.PermitirUbicacion : Textos.ConfirmarModificarUbicacion;
        if (!dialogos.Confirmar(mensaje, Textos.Confirmacion))
        {
            return;
        }
        Ubicacion = dialogos.Abrir<SeleccionUbicacionVistaModelo, Ubicacion>(Ubicacion) ?? Ubicacion;
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (!Validar())
        {
            dialogos.Mostrar(Textos.CamposObligatorios, TipoMensaje.Advertencia, Textos.Advertencia);
            return;
        }

        var solicitud = new SolicitudRegistro(Validador.Normalizar(Nombre), Telefono.Trim(), Correo.Trim(), Contrasena, Ubicacion);
        var resultado = await MientrasOcupadoAsync(() => cuentas.RegistrarAsync(solicitud));
        if (!resultado.Exito)
        {
            var mensaje = resultado.Error.Tipo == TipoError.Conflicto ? Textos.CorreoRegistrado : resultado.Error.Mensaje;
            dialogos.Mostrar(mensaje, TipoMensaje.Error, Textos.Error);
            return;
        }
        dialogos.Mostrar(Textos.RegistroExitoso, TipoMensaje.Informacion, Textos.Exito);
        Aceptar(true);
    }

    [RelayCommand]
    private void Regresar() => Cancelar();

    internal bool Validar()
    {
        NombreInvalido = !Validador.EsNombreValido(Nombre);
        CorreoInvalido = !Validador.EsCorreoValido(Correo);
        TelefonoInvalido = !Validador.EsTelefonoValido(Telefono);
        ContrasenaInvalida = !Validador.EsContrasenaValida(Contrasena);
        ConfirmacionInvalida = ContrasenaInvalida || !string.Equals(Contrasena, ConfirmacionContrasena, StringComparison.Ordinal);
        return !(NombreInvalido || CorreoInvalido || TelefonoInvalido || ContrasenaInvalida || ConfirmacionInvalida);
    }
}
