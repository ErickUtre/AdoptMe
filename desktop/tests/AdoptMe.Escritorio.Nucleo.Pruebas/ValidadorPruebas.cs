using AdoptMe.Escritorio.Nucleo.Validacion;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class ValidadorPruebas
{
    [Theory]
    [InlineData("Ana María", true)]
    [InlineData("José O'Neil", true)]
    [InlineData("Ana123", false)]
    [InlineData("   ", false)]
    public void ValidaNombres(string nombre, bool esperado) => Assert.Equal(esperado, Validador.EsNombreValido(nombre));

    [Theory]
    [InlineData("persona@correo.com", true)]
    [InlineData("sin-arroba.com", false)]
    public void ValidaCorreos(string correo, bool esperado) => Assert.Equal(esperado, Validador.EsCorreoValido(correo));

    [Theory]
    [InlineData("2281234567", true)]
    [InlineData("228123456", false)]
    [InlineData("22812345ab", false)]
    public void ValidaTelefonos(string telefono, bool esperado) => Assert.Equal(esperado, Validador.EsTelefonoValido(telefono));

    [Theory]
    [InlineData("Segura123", true)]
    [InlineData("segura123", false)]
    [InlineData("Segura1", false)]
    [InlineData("Segura 123", false)]
    public void ValidaReglasDeContrasena(string contrasena, bool esperado) => Assert.Equal(esperado, Validador.EsContrasenaValida(contrasena));
}
