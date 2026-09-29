package com.adoptme.movil.dominio;

import android.os.Parcel;
import android.os.Parcelable;

import androidx.annotation.NonNull;
import androidx.core.os.ParcelCompat;

public final class AdopcionCercana implements Parcelable {

    public static final Creator<AdopcionCercana> CREATOR = new Creator<>() {
        @Override
        public AdopcionCercana createFromParcel(Parcel origen) {
            return new AdopcionCercana(origen.readInt(), origen.readInt(), origen.readDouble(), origen.readDouble(),
                    ParcelCompat.readParcelable(origen, Mascota.class.getClassLoader(), Mascota.class));
        }

        @Override
        public AdopcionCercana[] newArray(int tamano) {
            return new AdopcionCercana[tamano];
        }
    };

    private final int adopcionId;
    private final int publicadorId;
    private final double latitud;
    private final double longitud;
    private final Mascota mascota;

    public AdopcionCercana(int adopcionId, int publicadorId, double latitud, double longitud, Mascota mascota) {
        this.adopcionId = adopcionId;
        this.publicadorId = publicadorId;
        this.latitud = latitud;
        this.longitud = longitud;
        this.mascota = mascota;
    }

    public int getAdopcionId() {
        return adopcionId;
    }

    public int getPublicadorId() {
        return publicadorId;
    }

    public double getLatitud() {
        return latitud;
    }

    public double getLongitud() {
        return longitud;
    }

    public Mascota getMascota() {
        return mascota;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(@NonNull Parcel destino, int opciones) {
        destino.writeInt(adopcionId);
        destino.writeInt(publicadorId);
        destino.writeDouble(latitud);
        destino.writeDouble(longitud);
        destino.writeParcelable(mascota, opciones);
    }
}
