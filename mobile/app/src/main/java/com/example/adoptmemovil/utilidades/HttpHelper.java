package com.example.adoptmemovil.utilidades;

import android.util.Log;

import com.example.adoptmemovil.modelo.ResultadoHttp;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HttpHelper {
    private static final String TAG = "HttpHelper";

    public static <T> void ejecutarHttp(Call<T> call, HttpCallback<T> callback) {
        Log.d(TAG, "Ejecutando llamada HTTP: " + call.request().url());

        call.enqueue(new Callback<T>() {
            @Override
            public void onResponse(Call<T> call, Response<T> response) {
                Log.d(TAG, "Respuesta HTTP recibida: " + response.code());

                ResultadoHttp<T> resultado = new ResultadoHttp<>();
                resultado.codigo = response.code();

                if (response.isSuccessful()) {
                    Log.i(TAG, "Respuesta exitosa");
                    resultado.exito = true;
                    resultado.contenido = response.body();
                } else if (response.code() == 401) {
                    Log.w(TAG, "Error 401: acceso no autorizado");
                    resultado.exito = false;
                    resultado.mensajeError = "No puedes acceder a este recurso";
                } else if (response.code() == 503) {
                    Log.w(TAG, "Error 503: error con la base de datos");
                    resultado.exito = false;
                    resultado.mensajeError = "Error con la base de datos";
                } else {
                    Log.w(TAG, "Error del servidor: código " + response.code());
                    resultado.exito = false;
                    resultado.mensajeError = "Error con el servidor";
                }

                callback.onRespuesta(resultado);
            }

            @Override
            public void onFailure(Call<T> call, Throwable t) {
                Log.e(TAG, "Falla en llamada HTTP: " + t.getMessage(), t);

                ResultadoHttp<T> resultado = new ResultadoHttp<>();
                resultado.exito = false;
                resultado.mensajeError = "Error con el servidor";
                callback.onRespuesta(resultado);
            }
        });
    }
}
