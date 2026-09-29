package com.adoptme.movil.dominio;

import android.os.Parcel;
import android.os.Parcelable;

import androidx.annotation.NonNull;

import com.google.gson.annotations.SerializedName;

public final class Ubicacion implements Parcelable {

    public static final Creator<Ubicacion> CREATOR = new Creator<>() {
        @Override
        public Ubicacion createFromParcel(Parcel origen) {
            return new Ubicacion(origen.readDouble(), origen.readDouble(), origen.readString(), origen.readString(), origen.readString());
        }

        @Override
        public Ubicacion[] newArray(int tamano) {
            return new Ubicacion[tamano];
        }
    };

    @SerializedName("Latitud")
    private final double latitud;
    @SerializedName("Longitud")
    private final double longitud;
    @SerializedName("Ciudad")
    private final String ciudad;
    @SerializedName("Estado")
    private final String estado;
    @SerializedName("Pais")
    private final String pais;

    public Ubicacion(double latitud, double longitud, String ciudad, String estado, String pais) {
        this.latitud = latitud;
        this.longitud = longitud;
        this.ciudad = ciudad;
        this.estado = estado;
        this.pais = pais;
    }

    public double getLatitud() {
        return latitud;
    }

    public double getLongitud() {
        return longitud;
    }

    public String getCiudad() {
        return ciudad;
    }

    public String getEstado() {
        return estado;
    }

    public String getPais() {
        return pais;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(@NonNull Parcel destino, int opciones) {
        destino.writeDouble(latitud);
        destino.writeDouble(longitud);
        destino.writeString(ciudad);
        destino.writeString(estado);
        destino.writeString(pais);
    }
}
