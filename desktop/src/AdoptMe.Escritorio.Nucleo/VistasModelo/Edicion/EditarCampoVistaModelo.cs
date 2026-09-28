using System.Globalization;
using AdoptMe.Escritorio.Nucleo.Modelos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;

public enum TipoCampo
{
    Texto,
    TextoLargo,
    Opciones,
    Edad
}

public sealed record CampoEditable(
    string Clave,
    string Etiqueta,
    TipoCampo Tipo,
    string ValorActual,
    int LongitudMaxima = 100,
    IReadOnlyList<string>? Opciones = null,
    Func<string, string?>? Validar = null);

public static class CamposMascota
{
    public static IReadOnlyList<string> Tamanos { get; } = ["Pequeño", "Mediano", "Grande"];

    public static IReadOnlyList<int> Anios { get; } = Enumerable.Range(0, 31).ToList();

    public static IReadOnlyList<int> Meses { get; } = Enumerable.Range(0, 12).ToList();

    public static string DescribirEdad(int anios, int meses) =>
        string.Create(CultureInfo.CurrentCulture, $"{anios} año(s) con {meses} mes(es)");

    public static CampoEditable Nombre(Mascota mascota) => new("Nombre", "Nombre", TipoCampo.Texto, mascota.Nombre, 45);

    public static CampoEditable Especie(Mascota mascota) => new("Especie", "Especie", TipoCampo.Texto, mascota.Especie, 50);

    public static CampoEditable Raza(Mascota mascota) => new("Raza", "Raza", TipoCampo.Texto, mascota.Raza, 100);

    public static CampoEditable Edad(Mascota mascota) => new("Edad", "Edad", TipoCampo.Edad, mascota.Edad);

    public static CampoEditable Sexo(Mascota mascota) => new("Sexo", "Sexo", TipoCampo.Opciones, mascota.Sexo, Opciones: SexosMascota.Todos);

    public static CampoEditable Tamano(Mascota mascota) => new("Tamaño", "Tamaño", TipoCampo.Opciones, mascota.Tamano, Opciones: Tamanos);

    public static CampoEditable Descripcion(Mascota mascota) =>
        new("Descripcion", "Descripción", TipoCampo.TextoLargo, mascota.Descripcion ?? string.Empty, 2000);
}

public sealed partial class EditarCampoVistaModelo : VistaModeloDialogo<string>
{
    [ObservableProperty]
    private CampoEditable? campo;

    [ObservableProperty]
    private string valorTexto = string.Empty;

    [ObservableProperty]
    private string? opcionSeleccionada;

    [ObservableProperty]
    private int anios;

    [ObservableProperty]
    private int meses;

    [ObservableProperty]
    private string? mensajeError;

    public override void Inicializar(object? parametro)
    {
        Campo = parametro as CampoEditable ?? throw new ArgumentException("Se esperaba un campo editable.", nameof(parametro));
        ValorTexto = Campo.ValorActual;
        OpcionSeleccionada = Campo.Opciones?.Contains(Campo.ValorActual) == true ? Campo.ValorActual : null;
    }

    [RelayCommand]
    private void Guardar()
    {
        if (Campo is null)
        {
            return;
        }
        var valor = ObtenerValor(Campo);
        MensajeError = string.IsNullOrWhiteSpace(valor)
            ? (Campo.Tipo == TipoCampo.Opciones ? Textos.SeleccionaOpcion : Textos.CampoVacio)
            : Campo.Validar?.Invoke(valor);
        if (MensajeError is null && valor is not null)
        {
            Aceptar(valor);
        }
    }

    private string? ObtenerValor(CampoEditable campoEditable) => campoEditable.Tipo switch
    {
        TipoCampo.Opciones => OpcionSeleccionada,
        TipoCampo.Edad => CamposMascota.DescribirEdad(Anios, Meses),
        _ => ValorTexto.Trim()
    };
}
