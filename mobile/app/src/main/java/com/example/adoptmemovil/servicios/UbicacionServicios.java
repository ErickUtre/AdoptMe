package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.Ubicacion;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Header;
import retrofit2.http.PUT;

public interface UbicacionServicios {
    @PUT("ubicaciones")
    Call<ResponseBody> actualizarUbicacion(@Body Ubicacion ubicacion, @Header("Authorization") String token);
}
