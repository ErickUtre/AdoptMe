package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.Usuario;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.Streaming;

public interface UsuarioServicios {
    @POST("usuarios")
    Call<ResponseBody> registrarUsuario(@Body Usuario nuevoUsuario);

    @PATCH("usuarios")
    Call<ResponseBody> actualizarPerfil(@Body Usuario usuario, @Header("Authorization") String token);

    @GET("usuarios/foto-perfil")
    @Streaming
    Call<ResponseBody> obtenerFotoPerfil(@Header("Authorization") String token);
}
