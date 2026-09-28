package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.SolicitudAdopcion;
import com.example.adoptmemovil.modelo.response.RegistroMascotaResponse;

import java.util.List;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.DELETE;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Path;

public interface SolicitudAdopcionServicios {

    @POST("solicitudAdopciones")
    Call<RegistroMascotaResponse> registrarSolicitudAdopcion(@Body SolicitudAdopcion solicitudAdopcion);

    @GET("solicitudAdopciones/aceptadas")
    Call<List<SolicitudAdopcion>> obtenerSolicitudesAceptadas();

    @GET("solicitudAdopciones/pendientes")
    Call<List<SolicitudAdopcion>> obtenerSolicitudesPendientes();

    @GET("solicitudAdopciones/por-publicador/{publicadorId}")
    Call<List<SolicitudAdopcion>> listarSolicitudesPorUsuario(@Path("publicadorId") int publicadorId);
    @DELETE("solicitudAdopciones/{id}")
    Call<ResponseBody> eliminarSolicitudAdopcion(@Path("id") int solicitudAdopcionId);
}
