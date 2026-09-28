package com.example.adoptmemovil.modelo.request;

public class IniciarSesionRequest {
    public String Correo;
    public String ContrasenaHash;

    public IniciarSesionRequest(String correo, String contrasenaHash) {
        this.Correo = correo;
        this.ContrasenaHash = contrasenaHash;
    }

    public String getContrasenaHash() {
        return ContrasenaHash;
    }

    public void setContrasenaHash(String contrasenaHash) {
        ContrasenaHash = contrasenaHash;
    }

    public String getCorreo() {
        return Correo;
    }

    public void setCorreo(String correo) {
        Correo = correo;
    }
}
