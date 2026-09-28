using AdoptMe.Escritorio.Nucleo.VistasModelo;

namespace AdoptMe.Escritorio.Nucleo.Presentacion;

public enum TipoMensaje
{
    Informacion,
    Advertencia,
    Error
}

public sealed record FiltroArchivo(string Descripcion, IReadOnlyList<string> Extensiones)
{
    public static FiltroArchivo Imagenes { get; } = new("Imágenes", [".jpg", ".jpeg", ".png"]);

    public static FiltroArchivo Videos { get; } = new("Videos MP4", [".mp4"]);

    public static FiltroArchivo Pdf { get; } = new("Documentos PDF", [".pdf"]);
}

public interface IDialogos
{
    void Mostrar(string mensaje, TipoMensaje tipo = TipoMensaje.Informacion, string? titulo = null);

    bool Confirmar(string mensaje, string? titulo = null);

    string? SeleccionarArchivo(FiltroArchivo filtro);

    string? SeleccionarDestino(FiltroArchivo filtro, string nombreSugerido);

    TResultado? Abrir<TVistaModelo, TResultado>(object? parametro = null)
        where TVistaModelo : VistaModeloDialogo<TResultado>;

    void MostrarImagen(byte[] imagen);

    void ReproducirVideo(byte[] video);
}

public interface INotificadorEmergente
{
    void Mostrar(string titulo, string mensaje);
}

public interface IDespachadorUi
{
    void Ejecutar(Action accion);
}

public interface IVentanas
{
    void MostrarInicioSesion();

    void MostrarPrincipal();
}

public interface IAlNavegar
{
    Task AlNavegarAsync(object? parametro);
}

public interface IAlSalir
{
    void AlSalir();
}
