package com.example.adoptmemovil.modelo;

import com.google.gson.annotations.SerializedName;

public class SolicitudAdopcion {

    @SerializedName("SolicitudAdopcionID")
    private int solicitudAdopcionID;

    @SerializedName("Estado")
    private boolean estado;

    @SerializedName("MascotaID")
    private int mascotaID;

    @SerializedName("PublicadorID")
    private int publicadorID;

    @SerializedName("UbicacionID")
    private int ubicacionID;

    @SerializedName("FechaSolicitud")
    private String fecha;

    @SerializedName("Mascota")
    private Mascota mascota;

    @SerializedName("Ubicacion")
    private Ubicacion ubicacion;

    // Getters y Setters
    public int getSolicitudAdopcionID() {
        return solicitudAdopcionID;
    }

    public void setSolicitudAdopcionID(int solicitudAdopcionID) {
        this.solicitudAdopcionID = solicitudAdopcionID;
    }

    public boolean isEstado() {
        return estado;
    }

    public void setEstado(boolean estado) {
        this.estado = estado;
    }

    public int getMascotaID() {
        return mascotaID;
    }

    public void setMascotaID(int mascotaID) {
        this.mascotaID = mascotaID;
    }

    public int getPublicadorID() {
        return publicadorID;
    }

    public void setPublicadorID(int publicadorID) {
        this.publicadorID = publicadorID;
    }

    public int getUbicacionID() {
        return ubicacionID;
    }

    public void setUbicacionID(int ubicacionID) {
        this.ubicacionID = ubicacionID;
    }

    public String getFecha() {
        return fecha;
    }

    public void setFecha(String fecha) {
        this.fecha = fecha;
    }

    public Mascota getMascota() {
        return mascota;
    }

    public void setMascota(Mascota mascota) {
        this.mascota = mascota;
    }

    public Ubicacion getUbicacion() {
        return ubicacion;
    }

    public void setUbicacion(Ubicacion ubicacion) {
        this.ubicacion = ubicacion;
    }
}
