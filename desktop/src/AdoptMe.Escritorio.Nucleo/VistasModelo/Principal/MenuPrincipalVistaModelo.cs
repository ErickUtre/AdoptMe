using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.Servicios.TiempoReal;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Notificaciones;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Perfil;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Reportes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Principal;

public sealed partial class MenuPrincipalVistaModelo : VistaModeloBase
{
    private readonly ISesionUsuario sesion;
    private readonly IServicioCuentas cuentas;
    private readonly ICanalNotificaciones canalNotificaciones;
    private readonly ICanalChat canalChat;
    private readonly INotificadorEmergente notificador;
    private readonly IDespachadorUi despachador;
    private readonly IVentanas ventanas;

    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private byte[]? fotoPerfil;

    [ObservableProperty]
    private bool hayNotificacionesNuevas;

    public MenuPrincipalVistaModelo(
        INavegador navegador,
        ISesionUsuario sesion,
        IServicioCuentas cuentas,
        ICanalNotificaciones canalNotificaciones,
        ICanalChat canalChat,
        INotificadorEmergente notificador,
        IDespachadorUi despachador,
        IVentanas ventanas)
    {
        Navegador = navegador;
        this.sesion = sesion;
        this.cuentas = cuentas;
        this.canalNotificaciones = canalNotificaciones;
        this.canalChat = canalChat;
        this.notificador = notificador;
        this.despachador = despachador;
        this.ventanas = ventanas;
    }

    public INavegador Navegador { get; }

    public bool EsAdministrador => sesion.EsAdministrador;

    public bool EsUsuario => !sesion.EsAdministrador;

    public async Task IniciarAsync()
    {
        sesion.PerfilActualizado += AlActualizarPerfil;
        sesion.FotoPerfilActualizada += AlActualizarFoto;
        NombreUsuario = sesion.Perfil?.Nombre ?? string.Empty;

        if (EsUsuario)
        {
            canalNotificaciones.NotificacionRecibida += AlRecibirNotificacion;
            canalNotificaciones.Iniciar();
            _ = canalChat.ConectarAsync();
        }

        await Navegador.NavegarAAsync<MapaAdopcionesVistaModelo>().ConfigureAwait(true);
        var foto = await cuentas.ObtenerFotoPerfilAsync().ConfigureAwait(true);
        if (foto.Exito)
        {
            sesion.ActualizarFotoPerfil(foto.Valor);
        }
    }

    [RelayCommand]
    private Task IrAMapaAsync() => Navegador.NavegarAAsync<MapaAdopcionesVistaModelo>();

    [RelayCommand]
    private Task IrARegistrarAdopcionAsync() => Navegador.NavegarAAsync<RegistroAdopcionVistaModelo>();

    [RelayCommand]
    private Task IrAAdopcionesPropiasAsync() => Navegador.NavegarAAsync<AdopcionesPropiasVistaModelo>();

    [RelayCommand]
    private Task IrAMensajesAsync() => Navegador.NavegarAAsync<ConversacionesVistaModelo>();

    [RelayCommand]
    private Task IrANotificacionesAsync()
    {
        HayNotificacionesNuevas = false;
        return Navegador.NavegarAAsync<NotificacionesVistaModelo>();
    }

    [RelayCommand]
    private Task IrAPerfilAsync() => Navegador.NavegarAAsync<PerfilVistaModelo>();

    [RelayCommand]
    private Task IrAReportesAsync() => Navegador.NavegarAAsync<ReportesVistaModelo>();

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        sesion.PerfilActualizado -= AlActualizarPerfil;
        sesion.FotoPerfilActualizada -= AlActualizarFoto;
        canalNotificaciones.NotificacionRecibida -= AlRecibirNotificacion;
        canalNotificaciones.Detener();
        await canalChat.DesconectarAsync().ConfigureAwait(true);
        Navegador.Reiniciar();
        sesion.Cerrar();
        ventanas.MostrarInicioSesion();
    }

    private void AlActualizarPerfil(object? remitente, EventArgs argumentos) =>
        despachador.Ejecutar(() => NombreUsuario = sesion.Perfil?.Nombre ?? string.Empty);

    private void AlActualizarFoto(object? remitente, EventArgs argumentos) =>
        despachador.Ejecutar(() => FotoPerfil = sesion.FotoPerfil);

    private void AlRecibirNotificacion(object? remitente, Notificacion notificacion) => despachador.Ejecutar(() =>
    {
        if (Navegador.Actual is NotificacionesVistaModelo)
        {
            return;
        }
        HayNotificacionesNuevas = true;
        notificador.Mostrar(notificacion.Titulo, notificacion.Mensaje);
    });
}
