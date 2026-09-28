package com.example.adoptmemovil.servicios;

import static com.example.adoptmemovil.utilidades.Constantes.DIRECCION_IP;

import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class ClienteAPI {
    private static final String BASE_URL = "http://" + DIRECCION_IP + ":8080/api/";
    private static Retrofit retrofit = null;

    public static Retrofit getRetrofit() {
        if (retrofit == null) {
            retrofit = new Retrofit.Builder()
                    .baseUrl(BASE_URL)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return retrofit;
    }

    public static AccesoServicios getAccesoServicios() {
        return getRetrofit().create(AccesoServicios.class);
    }

    public static UsuarioServicios getUsuarioServicios() {
        return getRetrofit().create(UsuarioServicios.class);
    }

    public static UbicacionServicios getUbicacionServicios() {
        return getRetrofit().create(UbicacionServicios.class);
    }

    public static SolicitudAdopcionServicios getSolicitudAdopcionServicios() {
        return getRetrofit().create(SolicitudAdopcionServicios.class);
    }

    public static SolicitudServicios getSolicitudServicios() {
        return getRetrofit().create(SolicitudServicios.class);
    }
}
