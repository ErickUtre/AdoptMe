package com.example.adoptmemovil.utilidades;

import com.example.adoptmemovil.modelo.Usuario;

public class UsuarioSingleton {
    private Usuario usuarioActual;
    private String token;
    private boolean logueado = false;

    private UsuarioSingleton() { }

    private static class Holder {
        private static final UsuarioSingleton INSTANCIA = new UsuarioSingleton();
    }

    public static UsuarioSingleton getInstancia() {
        return Holder.INSTANCIA;
    }

    public void iniciarSesion(Usuario usuario, String token) {
        this.usuarioActual = usuario;
        this.token = token;
        this.logueado = true;
    }

    public void cerrarSesion() {
        this.usuarioActual = null;
        this.token = null;
        this.logueado = false;
    }

    public boolean estaLogueado() {
        return logueado && usuarioActual != null && usuarioActual.getUsuarioID() > 0;
    }

    public Usuario getUsuarioActual() {
        return estaLogueado() ? usuarioActual : null;
    }

    public String getToken() {
        return estaLogueado() ? token : null;
    }
}
