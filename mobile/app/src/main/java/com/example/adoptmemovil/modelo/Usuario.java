package com.example.adoptmemovil.modelo;

import android.graphics.Bitmap;

import com.google.gson.annotations.SerializedName;

public class Usuario {
    @SerializedName("UsuarioID")
    private int usuarioID;
    @SerializedName("Nombre")
    private String nombre;
    @SerializedName("Telefono")
    private String telefono;
    @SerializedName("Ubicacion")
    private Ubicacion ubicacion;
    @SerializedName("Acceso")
    private Acceso acceso;
    private Bitmap fotoPerfil;

    public int getUsuarioID() {
        return usuarioID;
    }

    public void setUsuarioID(int usuarioID) {
        this.usuarioID = usuarioID;
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public String getTelefono() {
        return telefono;
    }

    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    public Ubicacion getUbicacion() {
        return ubicacion;
    }

    public void setUbicacion(Ubicacion ubicacion) {
        this.ubicacion = ubicacion;
    }

    public Acceso getAcceso() {
        return acceso;
    }

    public void setAcceso(Acceso acceso) {
        this.acceso = acceso;
    }

    public Bitmap getFotoPerfil() { return fotoPerfil; }

    public void setFotoPerfil(Bitmap fotoPerfil) { this.fotoPerfil = fotoPerfil; }

    public boolean hayFoto() { return fotoPerfil != null; }

    public void limpiarFoto() { fotoPerfil = null; }
}
