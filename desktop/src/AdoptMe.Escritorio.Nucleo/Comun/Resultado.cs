using System.Diagnostics.CodeAnalysis;

namespace AdoptMe.Escritorio.Nucleo.Comun;

public enum TipoError
{
    Validacion,
    NoAutenticado,
    Prohibido,
    NoEncontrado,
    Conflicto,
    ServicioNoDisponible,
    Conexion,
    Desconocido
}

public sealed record ErrorOperacion(TipoError Tipo, string Mensaje);

public class Resultado
{
    protected Resultado(ErrorOperacion? error)
    {
        Error = error;
    }

    public ErrorOperacion? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool Exito => Error is null;

    public static Resultado Correcto() => new(null);

    public static Resultado Fallo(ErrorOperacion error) => new(error);

    public static Resultado<T> Correcto<T>(T valor) => new(valor, null);

    public static Resultado<T> Fallo<T>(ErrorOperacion error) => new(default, error);
}

public sealed class Resultado<T> : Resultado
{
    internal Resultado(T? valor, ErrorOperacion? error)
        : base(error)
    {
        Valor = valor;
    }

    public T? Valor { get; }

    public new ErrorOperacion? Error => base.Error;

    [MemberNotNullWhen(true, nameof(Valor))]
    [MemberNotNullWhen(false, nameof(Error))]
    public new bool Exito => base.Exito;
}
