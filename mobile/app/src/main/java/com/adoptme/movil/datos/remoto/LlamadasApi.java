package com.adoptme.movil.datos.remoto;

import androidx.annotation.NonNull;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.comun.TipoError;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonParser;

import java.io.IOException;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public final class LlamadasApi {

    private LlamadasApi() {
    }

    public static <T> void ejecutar(Call<T> llamada, AlTerminar<T> alTerminar) {
        llamada.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<T> llamadaRealizada, @NonNull Response<T> respuesta) {
                if (respuesta.isSuccessful()) {
                    alTerminar.con(Resultado.exito(respuesta.body()));
                    return;
                }
                alTerminar.con(Resultado.fallo(TipoError.desdeCodigoHttp(respuesta.code()), leerMensaje(respuesta.errorBody())));
            }

            @Override
            public void onFailure(@NonNull Call<T> llamadaRealizada, @NonNull Throwable causa) {
                alTerminar.con(Resultado.fallo(TipoError.CONEXION, null));
            }
        });
    }

    private static String leerMensaje(ResponseBody cuerpo) {
        if (cuerpo == null) {
            return null;
        }
        try (cuerpo) {
            JsonObject json = JsonParser.parseString(cuerpo.string()).getAsJsonObject();
            return json.has("error") ? json.get("error").getAsString() : null;
        } catch (IOException | JsonParseException | IllegalStateException excepcion) {
            return null;
        }
    }
}
