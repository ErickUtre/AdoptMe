package com.example.adoptmemovil.modelo;

import com.google.gson.annotations.SerializedName;

public class Solicitud {

    @SerializedName("SolicitudID")
    private int solicitudID;

    @SerializedName("AdopcionID")
    private int adopcionID;

    @SerializedName("NombreAdoptante")
    private String nombreUsuarioAdoptante;

    // Getters y Setters

    public int getSolicitudID() {
        return solicitudID;
    }

    public int getAdopcionID() {
        return adopcionID;
    }

    public void setSolicitudID(int solicitudID) {
        this.solicitudID = solicitudID;
    }

    public String getNombreUsuarioAdoptante() {
        return nombreUsuarioAdoptante;
    }

    public void setNombreUsuarioAdoptante(String nombreUsuarioAdoptante) {
        this.nombreUsuarioAdoptante = nombreUsuarioAdoptante;
    }
}
