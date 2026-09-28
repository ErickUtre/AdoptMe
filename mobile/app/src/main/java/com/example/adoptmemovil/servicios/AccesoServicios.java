package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.Acceso;
import com.example.adoptmemovil.modelo.request.IniciarSesionRequest;
import com.example.adoptmemovil.modelo.response.IniciarSesionResponse;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Header;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.PUT;

public interface AccesoServicios {
    @POST("acceso/iniciar-sesion")
    Call<IniciarSesionResponse> iniciarSesion(@Body IniciarSesionRequest request);

    @PATCH("acceso")
    Call<ResponseBody> actualizarAcceso(@Body Acceso acceso, @Header("Authorization") String token);
}
