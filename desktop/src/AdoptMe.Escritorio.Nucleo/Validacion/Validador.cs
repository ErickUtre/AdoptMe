using System.Text.RegularExpressions;

namespace AdoptMe.Escritorio.Nucleo.Validacion;

public static partial class Validador
{
    private const int LongitudMinimaContrasena = 8;
    private const int DigitosMinimosContrasena = 2;

    public static bool EsNombreValido(string? nombre) =>
        !string.IsNullOrWhiteSpace(nombre) && PatronNombre().IsMatch(Normalizar(nombre));

    public static bool EsCorreoValido(string? correo) =>
        !string.IsNullOrWhiteSpace(correo) && PatronCorreo().IsMatch(correo.Trim());

    public static bool EsTelefonoValido(string? telefono) =>
        !string.IsNullOrWhiteSpace(telefono) && PatronTelefono().IsMatch(telefono.Trim());

    public static bool EsContrasenaValida(string? contrasena) =>
        !string.IsNullOrEmpty(contrasena)
        && contrasena.Length >= LongitudMinimaContrasena
        && !contrasena.Any(char.IsWhiteSpace)
        && contrasena.Any(char.IsUpper)
        && contrasena.Count(char.IsDigit) >= DigitosMinimosContrasena;

    public static string Normalizar(string texto)
    {
        ArgumentNullException.ThrowIfNull(texto);
        return EspaciosMultiples().Replace(texto.Trim(), " ");
    }

    [GeneratedRegex(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ'’-]+)*$", RegexOptions.None, 1000)]
    private static partial Regex PatronNombre();

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.None, 1000)]
    private static partial Regex PatronCorreo();

    [GeneratedRegex(@"^\d{10}$", RegexOptions.None, 1000)]
    private static partial Regex PatronTelefono();

    [GeneratedRegex(@"\s+", RegexOptions.None, 1000)]
    private static partial Regex EspaciosMultiples();
}
