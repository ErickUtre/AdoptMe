using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Sesion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Acceso;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Edicion;
using AdoptMe.Escritorio.Nucleo.VistasModelo.Ubicaciones;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class InicioSesionVistaModeloPruebas
{
    private static Modelos.Sesion SesionDePrueba(bool esAdmin) =>
        new("token", esAdmin, new Perfil { UsuarioID = 7, Nombre = "Ana", Acceso = new DatosAcceso("ana@correo.com", esAdmin) });

    [Fact]
    public async Task IniciaLaSesionYAbreLaVentanaPrincipal()
    {
        var cuentas = new CuentasFalsas { RespuestaInicioSesion = Resultado.Correcto(SesionDePrueba(true)) };
        var sesion = new SesionUsuario();
        var ventanas = new VentanasFalsas();
        var vistaModelo = new InicioSesionVistaModelo(cuentas, sesion, new DialogosFalsos(), ventanas)
        {
            Correo = "ana@correo.com",
            Contrasena = "Segura123"
        };

        await vistaModelo.IniciarSesionCommand.ExecuteAsync(null);

        Assert.True(sesion.EstaAutenticada);
        Assert.True(sesion.EsAdministrador);
        Assert.Equal(1, ventanas.VecesPrincipal);
        Assert.Empty(vistaModelo.Contrasena);
    }

    [Fact]
    public async Task InformaCredencialesIncorrectasSinAbrirLaVentanaPrincipal()
    {
        var cuentas = new CuentasFalsas
        {
            RespuestaInicioSesion = Resultado.Fallo<Modelos.Sesion>(new ErrorOperacion(TipoError.NoAutenticado, "401"))
        };
        var dialogos = new DialogosFalsos();
        var ventanas = new VentanasFalsas();
        var vistaModelo = new InicioSesionVistaModelo(cuentas, new SesionUsuario(), dialogos, ventanas)
        {
            Correo = "ana@correo.com",
            Contrasena = "Incorrecta1"
        };

        await vistaModelo.IniciarSesionCommand.ExecuteAsync(null);

        Assert.Equal(0, ventanas.VecesPrincipal);
        Assert.Equal((Textos.CredencialesIncorrectas, TipoMensaje.Error), Assert.Single(dialogos.Mensajes));
    }

    [Fact]
    public void NoPermiteIniciarSesionConCamposVacios()
    {
        var vistaModelo = new InicioSesionVistaModelo(new CuentasFalsas(), new SesionUsuario(), new DialogosFalsos(), new VentanasFalsas());
        Assert.False(vistaModelo.IniciarSesionCommand.CanExecute(null));
    }
}

public class RegistroUsuarioVistaModeloPruebas
{
    [Fact]
    public async Task MarcaLosCamposInvalidosYNoEnviaElRegistro()
    {
        var cuentas = new CuentasFalsas();
        var vistaModelo = new RegistroUsuarioVistaModelo(cuentas, new DialogosFalsos())
        {
            Nombre = "Ana1",
            Correo = "correo-invalido",
            Telefono = "123",
            Contrasena = "Segura123",
            ConfirmacionContrasena = "Distinta123"
        };

        await vistaModelo.RegistrarCommand.ExecuteAsync(null);

        Assert.True(vistaModelo.NombreInvalido);
        Assert.True(vistaModelo.CorreoInvalido);
        Assert.True(vistaModelo.TelefonoInvalido);
        Assert.False(vistaModelo.ContrasenaInvalida);
        Assert.True(vistaModelo.ConfirmacionInvalida);
        Assert.Null(cuentas.UltimoRegistro);
    }

    [Fact]
    public async Task EnviaElRegistroNormalizadoYCierraElDialogo()
    {
        var cuentas = new CuentasFalsas();
        var vistaModelo = new RegistroUsuarioVistaModelo(cuentas, new DialogosFalsos())
        {
            Nombre = "  Ana   María ",
            Correo = "ana@correo.com",
            Telefono = "2281234567",
            Contrasena = "Segura123",
            ConfirmacionContrasena = "Segura123"
        };
        bool? cerrado = null;
        vistaModelo.CierreSolicitado += (_, aceptado) => cerrado = aceptado;

        await vistaModelo.RegistrarCommand.ExecuteAsync(null);

        Assert.Equal("Ana María", cuentas.UltimoRegistro?.Nombre);
        Assert.True(cerrado);
    }
}

public class EditarCampoVistaModeloPruebas
{
    [Fact]
    public void ComponeLaEdadConAniosYMeses()
    {
        var vistaModelo = new EditarCampoVistaModelo();
        vistaModelo.Inicializar(CamposMascota.Edad(new Mascota()));
        vistaModelo.Anios = 2;
        vistaModelo.Meses = 5;

        vistaModelo.GuardarCommand.Execute(null);

        Assert.Equal("2 año(s) con 5 mes(es)", vistaModelo.Resultado);
    }

    [Fact]
    public void AplicaLaValidacionPersonalizada()
    {
        var vistaModelo = new EditarCampoVistaModelo();
        vistaModelo.Inicializar(new CampoEditable("Telefono", "Teléfono", TipoCampo.Texto, string.Empty, Validar: _ => Textos.TelefonoInvalido));
        vistaModelo.ValorTexto = "123";

        vistaModelo.GuardarCommand.Execute(null);

        Assert.Equal(Textos.TelefonoInvalido, vistaModelo.MensajeError);
        Assert.Null(vistaModelo.Resultado);
    }
}

public class SeleccionUbicacionVistaModeloPruebas
{
    [Theory]
    [InlineData("México", true)]
    [InlineData("Mexico", true)]
    [InlineData("Guatemala", false)]
    [InlineData(null, false)]
    public void SoloAceptaUbicacionesEnMexico(string? pais, bool esperado) =>
        Assert.Equal(esperado, SeleccionUbicacionVistaModelo.EsDeMexico(new Ubicacion { Pais = pais }));
}
