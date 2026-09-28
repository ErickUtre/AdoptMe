package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.Mascota;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.Path;

public interface MascotaServicios {
    @POST("mascotas")
    Call<ResponseBody> registrarMascota(@Body Mascota mascota);
    @GET("mascotas/{id}/foto")
    Call<ResponseBody> obtenerFotoMascota(@Header("Authorization") String token, @Path("id") int id);
    @GET("mascotas/{id}/video")
    Call<ResponseBody> obtenerVideoMascota(@Header("Authorization") String token, @Path("id") int id);
}

