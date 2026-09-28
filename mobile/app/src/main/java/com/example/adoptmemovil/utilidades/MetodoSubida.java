package com.example.adoptmemovil.utilidades;

import io.grpc.stub.StreamObserver;
import multimedia.ChunkArchivo;
import multimedia.RespuestaGeneral;

public interface MetodoSubida {
    StreamObserver<ChunkArchivo> aplicar(StreamObserver<RespuestaGeneral> respuesta);
}
