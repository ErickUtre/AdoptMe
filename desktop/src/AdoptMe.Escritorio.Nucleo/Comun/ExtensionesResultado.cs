namespace AdoptMe.Escritorio.Nucleo.Comun;

public static class ExtensionesResultado
{
    public static Resultado<IReadOnlyList<T>> ComoSoloLectura<T>(this Resultado<List<T>> resultado)
    {
        ArgumentNullException.ThrowIfNull(resultado);
        return resultado.Exito
            ? Resultado.Correcto<IReadOnlyList<T>>(resultado.Valor)
            : Resultado.Fallo<IReadOnlyList<T>>(resultado.Error);
    }

    public static Resultado SinValor<T>(this Resultado<T> resultado)
    {
        ArgumentNullException.ThrowIfNull(resultado);
        return resultado.Exito ? Resultado.Correcto() : Resultado.Fallo(resultado.Error);
    }
}
