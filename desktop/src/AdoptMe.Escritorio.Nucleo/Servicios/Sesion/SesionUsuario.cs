using AdoptMe.Escritorio.Nucleo.Modelos;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Sesion;

public interface ISesionUsuario
{
    event EventHandler? PerfilActualizado;

    event EventHandler? FotoPerfilActualizada;

    Perfil? Perfil { get; }

    string? Token { get; }

    bool EsAdministrador { get; }

    bool EstaAutenticada { get; }

    byte[]? FotoPerfil { get; }

    Coordenadas? UbicacionTemporal { get; set; }

    int UsuarioId { get; }

    void Iniciar(Modelos.Sesion sesion);

    void Cerrar();

    void ActualizarPerfil(Perfil perfil);

    void ActualizarFotoPerfil(byte[]? foto);
}

public sealed class SesionUsuario : ISesionUsuario
{
    public event EventHandler? PerfilActualizado;

    public event EventHandler? FotoPerfilActualizada;

    public Perfil? Perfil { get; private set; }

    public string? Token { get; private set; }

    public bool EsAdministrador { get; private set; }

    public bool EstaAutenticada => !string.IsNullOrEmpty(Token) && Perfil is not null;

    public byte[]? FotoPerfil { get; private set; }

    public Coordenadas? UbicacionTemporal { get; set; }

    public int UsuarioId => Perfil?.UsuarioID ?? throw new InvalidOperationException("No hay una sesión activa.");

    public void Iniciar(Modelos.Sesion sesion)
    {
        ArgumentNullException.ThrowIfNull(sesion);
        Token = sesion.Token;
        EsAdministrador = sesion.EsAdmin;
        Perfil = sesion.Usuario;
        FotoPerfil = null;
        UbicacionTemporal = null;
        PerfilActualizado?.Invoke(this, EventArgs.Empty);
    }

    public void Cerrar()
    {
        Token = null;
        Perfil = null;
        EsAdministrador = false;
        FotoPerfil = null;
        UbicacionTemporal = null;
    }

    public void ActualizarPerfil(Perfil perfil)
    {
        Perfil = perfil;
        PerfilActualizado?.Invoke(this, EventArgs.Empty);
    }

    public void ActualizarFotoPerfil(byte[]? foto)
    {
        FotoPerfil = foto;
        FotoPerfilActualizada?.Invoke(this, EventArgs.Empty);
    }
}
