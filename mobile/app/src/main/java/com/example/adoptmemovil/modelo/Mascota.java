package com.example.adoptmemovil.modelo;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

public class Mascota implements Parcelable {

    @SerializedName("MascotaID")
    public int mascotaID;

    @SerializedName("Nombre")
    public String nombre;

    @SerializedName("Especie")
    public String especie;

    @SerializedName("Raza")
    public String raza;

    @SerializedName("Edad")
    public String edad;

    @SerializedName("Sexo")
    public String sexo;

    @SerializedName("Tamaño")
    public String tamaño;

    @SerializedName("Descripcion")
    public String descripcion;

    public Mascota() {}

    public Mascota(int mascotaID, String nombre, String especie, String raza, String edad,
                   String sexo, String tamaño, String descripcion) {
        this.mascotaID = mascotaID;
        this.nombre = nombre;
        this.especie = especie;
        this.raza = raza;
        this.edad = edad;
        this.sexo = sexo;
        this.tamaño = tamaño;
        this.descripcion = descripcion;
    }

    public int getMascotaID() { return mascotaID; }
    public void setMascotaID(int mascotaID) { this.mascotaID = mascotaID; }

    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }

    public String getEspecie() { return especie; }
    public void setEspecie(String especie) { this.especie = especie; }

    public String getRaza() { return raza; }
    public void setRaza(String raza) { this.raza = raza; }

    public String getEdad() { return edad; }
    public void setEdad(String edad) { this.edad = edad; }

    public String getSexo() { return sexo; }
    public void setSexo(String sexo) { this.sexo = sexo; }

    public String getTamaño() { return tamaño; }
    public void setTamaño(String tamaño) { this.tamaño = tamaño; }

    public String getDescripcion() { return descripcion; }
    public void setDescripcion(String descripcion) { this.descripcion = descripcion; }

    protected Mascota(Parcel in) {
        mascotaID = in.readInt();
        nombre = in.readString();
        especie = in.readString();
        raza = in.readString();
        edad = in.readString();
        sexo = in.readString();
        tamaño = in.readString();
        descripcion = in.readString();
    }

    public static final Creator<Mascota> CREATOR = new Creator<Mascota>() {
        @Override
        public Mascota createFromParcel(Parcel in) {
            return new Mascota(in);
        }

        @Override
        public Mascota[] newArray(int size) {
            return new Mascota[size];
        }
    };

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeInt(mascotaID);
        dest.writeString(nombre);
        dest.writeString(especie);
        dest.writeString(raza);
        dest.writeString(edad);
        dest.writeString(sexo);
        dest.writeString(tamaño);
        dest.writeString(descripcion);
    }

    @Override
    public int describeContents() {
        return 0;
    }
}
