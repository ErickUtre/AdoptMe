package com.adoptme.movil.datos.remoto;

import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public final class ClienteRest {

    private static final long TIEMPO_ESPERA_SEGUNDOS = 30;

    private final Retrofit retrofit;

    public ClienteRest(String urlBase, SesionUsuario sesion) {
        OkHttpClient http = new OkHttpClient.Builder()
                .connectTimeout(TIEMPO_ESPERA_SEGUNDOS, TimeUnit.SECONDS)
                .readTimeout(TIEMPO_ESPERA_SEGUNDOS, TimeUnit.SECONDS)
                .addInterceptor(cadena -> {
                    Request.Builder peticion = cadena.request().newBuilder();
                    String token = sesion.getToken();
                    if (token != null) {
                        peticion.header("Authorization", "Bearer " + token);
                    }
                    return cadena.proceed(peticion.build());
                })
                .build();
        Gson gson = new GsonBuilder().create();
        retrofit = new Retrofit.Builder()
                .baseUrl(urlBase)
                .client(http)
                .addConverterFactory(GsonConverterFactory.create(gson))
                .build();
    }

    public <T> T crear(Class<T> servicio) {
        return retrofit.create(servicio);
    }

    public String getUrlBase() {
        return retrofit.baseUrl().toString();
    }
}
