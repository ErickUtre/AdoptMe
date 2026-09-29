package com.adoptme.movil.dominio;

import android.os.Parcel;
import android.os.Parcelable;

import androidx.annotation.NonNull;

import com.google.gson.annotations.SerializedName;

import java.util.Objects;

public final class Mascota implements Parcelable {

    public static final Creator<Mascota> CREATOR = new Creator<>() {
        @Override
        public Mascota createFromParcel(Parcel origen) {
            return new Mascota(origen.readInt(), origen.readString(), origen.readString(), origen.readString(),
                    origen.readString(), origen.readString(), origen.readString(), origen.readString());
        }

        @Override
        public Mascota[] newArray(int tamano) {
            return new Mascota[tamano];
        }
    };

    @SerializedName("MascotaID")
    private final int mascotaId;
    @SerializedName("Nombre")
    private final String nombre;
    @SerializedName("Especie")
    private final String especie;
    @SerializedName("Raza")
    private final String raza;
    @SerializedName("Edad")
    private final String edad;
    @SerializedName("Sexo")
    private final String sexo;
    @SerializedName("Tamaño")
    private final String tamano;
    @SerializedName("Descripcion")
    private final String descripcion;

    public Mascota(int mascotaId, String nombre, String especie, String raza, String edad, String sexo, String tamano, String descripcion) {
        this.mascotaId = mascotaId;
        this.nombre = nombre;
        this.especie = especie;
        this.raza = raza;
        this.edad = edad;
        this.sexo = sexo;
        this.tamano = tamano;
        this.descripcion = descripcion;
    }

    public int getMascotaId() {
        return mascotaId;
    }

    public String getNombre() {
        return nombre;
    }

    public String getEspecie() {
        return especie;
    }

    public String getRaza() {
        return raza;
    }

    public String getEdad() {
        return edad;
    }

    public String getSexo() {
        return sexo;
    }

    public String getTamano() {
        return tamano;
    }

    public String getDescripcion() {
        return descripcion;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(@NonNull Parcel destino, int opciones) {
        destino.writeInt(mascotaId);
        destino.writeString(nombre);
        destino.writeString(especie);
        destino.writeString(raza);
        destino.writeString(edad);
        destino.writeString(sexo);
        destino.writeString(tamano);
        destino.writeString(descripcion);
    }

    @Override
    public boolean equals(Object otro) {
        if (this == otro) {
            return true;
        }
        if (!(otro instanceof Mascota)) {
            return false;
        }
        Mascota mascota = (Mascota) otro;
        return mascotaId == mascota.mascotaId
                && Objects.equals(nombre, mascota.nombre)
                && Objects.equals(especie, mascota.especie)
                && Objects.equals(raza, mascota.raza)
                && Objects.equals(edad, mascota.edad)
                && Objects.equals(sexo, mascota.sexo)
                && Objects.equals(tamano, mascota.tamano)
                && Objects.equals(descripcion, mascota.descripcion);
    }

    @Override
    public int hashCode() {
        return Objects.hash(mascotaId, nombre, especie, raza, edad, sexo, tamano, descripcion);
    }
}
