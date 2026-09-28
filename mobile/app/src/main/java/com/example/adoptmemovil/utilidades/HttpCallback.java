package com.example.adoptmemovil.utilidades;

import com.example.adoptmemovil.modelo.ResultadoHttp;

public interface HttpCallback<T> {
    void onRespuesta(ResultadoHttp<T> resultadoHttp);
}
