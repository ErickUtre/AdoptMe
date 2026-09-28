using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Grpc;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Mapa;

public sealed partial class MapaAdopcionesVistaModelo(
    IServicioMapa mapa,
    IServicioAdopciones adopciones,
    IServicioGeolocalizacion geolocalizacion,
    ISesionUsuario sesion,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    public const double ZoomMinimoMarcadores = 12;
    public const double ZoomDetalle = 17;
    public const double ZoomPais = 6;
    private const double DesplazamientoMinimoGrados = 0.01;

    public static Coordenadas CentroPorDefecto { get; } = new(19.4326, -99.1332);

    private Coordenadas? ultimaConsulta;

    [ObservableProperty]
    private Coordenadas centro = CentroPorDefecto;

    [ObservableProperty]
    private double zoom = ZoomPais;

    [ObservableProperty]
    private Coordenadas? ubicacionUsuario;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TextoAlternarUbicacion))]
    private bool mostrandoUbicacionTemporal;

    public ObservableCollection<AdopcionCercana> Adopciones { get; } = [];

    public string TextoAlternarUbicacion => MostrandoUbicacionTemporal ? Textos.MostrarUbicacionRegistrada : Textos.MostrarUbicacionActual;

    public async Task AlNavegarAsync(object? parametro)
    {
        MostrandoUbicacionTemporal = sesion.UbicacionTemporal is not null;
        var posicion = sesion.UbicacionTemporal ?? sesion.Perfil?.Ubicacion?.Coordenadas;
        UbicacionUsuario = posicion ?? CentroPorDefecto;

        if (parametro is int adopcionId && await EnfocarAdopcionAsync(adopcionId).ConfigureAwait(true))
        {
            return;
        }
        await CentrarEnAsync(UbicacionUsuario).ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task CargarCercanasAsync(Coordenadas nuevoCentro)
    {
        if (ultimaConsulta is not null
            && Math.Abs(nuevoCentro.Latitud - ultimaConsulta.Latitud) < DesplazamientoMinimoGrados
            && Math.Abs(nuevoCentro.Longitud - ultimaConsulta.Longitud) < DesplazamientoMinimoGrados)
        {
            return;
        }
        ultimaConsulta = nuevoCentro;
        var resultado = await mapa.ObtenerCercanasAsync(nuevoCentro).ConfigureAwait(true);
        if (!resultado.Exito)
        {
            return;
        }
        Adopciones.Clear();
        foreach (var adopcion in resultado.Valor)
        {
            Adopciones.Add(adopcion);
        }
    }

    [RelayCommand]
    private Task VerDetalleAsync(AdopcionCercana adopcion) => navegador.NavegarAAsync<DetalleAdopcionVistaModelo>(adopcion);

    [RelayCommand]
    private async Task AlternarUbicacionAsync()
    {
        if (MostrandoUbicacionTemporal)
        {
            sesion.UbicacionTemporal = null;
            MostrandoUbicacionTemporal = false;
            UbicacionUsuario = sesion.Perfil?.Ubicacion?.Coordenadas ?? CentroPorDefecto;
            await CentrarEnAsync(UbicacionUsuario).ConfigureAwait(true);
            return;
        }

        var aproximada = await MientrasOcupadoAsync(geolocalizacion.ObtenerUbicacionAproximadaAsync).ConfigureAwait(true);
        if (!aproximada.Exito)
        {
            dialogos.Mostrar(aproximada.Error.Mensaje, TipoMensaje.Error, Textos.Error);
            return;
        }
        sesion.UbicacionTemporal = aproximada.Valor.Coordenadas;
        MostrandoUbicacionTemporal = true;
        UbicacionUsuario = aproximada.Valor.Coordenadas;
        await CentrarEnAsync(UbicacionUsuario).ConfigureAwait(true);
    }

    private async Task CentrarEnAsync(Coordenadas destino)
    {
        Zoom = ZoomDetalle;
        Centro = destino;
        ultimaConsulta = null;
        await CargarCercanasAsync(destino).ConfigureAwait(true);
    }

    private async Task<bool> EnfocarAdopcionAsync(int adopcionId)
    {
        var detalle = await adopciones.ObtenerDetalleAsync(adopcionId).ConfigureAwait(true);
        if (!detalle.Exito || detalle.Valor.Ubicacion is null || detalle.Valor.Mascota is null)
        {
            return false;
        }
        var coordenadas = detalle.Valor.Ubicacion.Coordenadas;
        await CentrarEnAsync(coordenadas).ConfigureAwait(true);
        if (Adopciones.All(adopcion => adopcion.AdopcionId != adopcionId))
        {
            Adopciones.Add(new AdopcionCercana(adopcionId, detalle.Valor.PublicadorID, 0, coordenadas, detalle.Valor.Mascota));
        }
        return true;
    }
}
