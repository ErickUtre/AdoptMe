using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Solicitudes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Adopciones;

public sealed partial class ElementoAdopcionVistaModelo(Adopcion adopcion) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstadoTexto))]
    [NotifyPropertyChangedFor(nameof(FueAdoptada))]
    private Adopcion adopcion = adopcion;

    [ObservableProperty]
    private byte[]? foto;

    public Mascota Mascota => Adopcion.Mascota ?? new Mascota();

    public bool FueAdoptada => Adopcion.Estado;

    public string EstadoTexto => Adopcion.Estado ? Textos.Adoptado : Textos.Disponible;

    public bool Coincide(string filtro) =>
        string.IsNullOrWhiteSpace(filtro)
        || Mascota.Nombre.Contains(filtro.Trim(), StringComparison.CurrentCultureIgnoreCase);
}

public sealed partial class AdopcionesPropiasVistaModelo(
    IServicioAdopciones adopciones,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    private readonly List<ElementoAdopcionVistaModelo> todas = [];

    [ObservableProperty]
    private string filtro = string.Empty;

    [ObservableProperty]
    private bool sinResultados;

    public ObservableCollection<ElementoAdopcionVistaModelo> Visibles { get; } = [];

    public async Task AlNavegarAsync(object? parametro)
    {
        var resultado = await MientrasOcupadoAsync(adopciones.ListarPropiasAsync).ConfigureAwait(true);
        if (!Informar(dialogos, resultado, out var lista))
        {
            return;
        }
        todas.Clear();
        todas.AddRange(lista.Select(adopcion => new ElementoAdopcionVistaModelo(adopcion)));
        Filtrar();
        await Task.WhenAll(todas.Select(CargarFotoAsync)).ConfigureAwait(true);
    }

    partial void OnFiltroChanged(string value) => Filtrar();

    [RelayCommand]
    private Task ConsultarAsync(ElementoAdopcionVistaModelo elemento) =>
        navegador.NavegarAAsync<EdicionAdopcionVistaModelo>(elemento.Adopcion);

    [RelayCommand]
    private async Task EliminarAsync(ElementoAdopcionVistaModelo elemento)
    {
        if (!dialogos.Confirmar(Textos.ConfirmarEliminarAdopcion(elemento.Mascota.Nombre), Textos.Confirmacion))
        {
            return;
        }
        var resultado = await MientrasOcupadoAsync(() => adopciones.EliminarAsync(elemento.Adopcion.AdopcionID)).ConfigureAwait(true);
        if (Informar(dialogos, resultado, Textos.AdopcionEliminada))
        {
            todas.Remove(elemento);
            Filtrar();
        }
    }

    [RelayCommand]
    private async Task VerSolicitudesAsync(ElementoAdopcionVistaModelo elemento)
    {
        var desenlace = dialogos.Abrir<SolicitudesVistaModelo, DesenlaceSolicitudes>(elemento.Adopcion.AdopcionID);
        if (desenlace is null)
        {
            return;
        }
        if (desenlace.AdopcionAceptada)
        {
            elemento.Adopcion = elemento.Adopcion with { Estado = true };
        }
        if (desenlace.ContactarUsuarioId is int usuarioId)
        {
            await navegador.NavegarAAsync<ChatVistaModelo>(usuarioId).ConfigureAwait(true);
        }
    }

    private async Task CargarFotoAsync(ElementoAdopcionVistaModelo elemento)
    {
        var foto = await adopciones.ObtenerFotoMascotaAsync(elemento.Adopcion.MascotaID).ConfigureAwait(true);
        if (foto.Exito)
        {
            elemento.Foto = foto.Valor;
        }
    }

    private void Filtrar()
    {
        Visibles.Clear();
        foreach (var elemento in todas.Where(elemento => elemento.Coincide(Filtro)))
        {
            Visibles.Add(elemento);
        }
        SinResultados = Visibles.Count == 0;
    }
}
