package com.adoptme.movil.datos.remoto.grpc;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.datos.repositorios.MapaRepositorio;
import com.adoptme.movil.dominio.AdopcionCercana;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.grpc.ubicacion.AdopcionesCercanas;
import com.adoptme.movil.grpc.ubicacion.Coordenadas;
import com.adoptme.movil.grpc.ubicacion.MascotaCercana;
import com.adoptme.movil.grpc.ubicacion.ServicioUbicacionGrpc;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executor;
import java.util.concurrent.TimeUnit;

import io.grpc.StatusRuntimeException;

public final class FuenteMapaGrpc implements MapaRepositorio {

    private static final long LIMITE_SEGUNDOS = 10;

    private final CanalGrpc canal;
    private final Executor ejecutor;

    public FuenteMapaGrpc(CanalGrpc canal, Executor ejecutor) {
        this.canal = canal;
        this.ejecutor = ejecutor;
    }

    @Override
    public void obtenerCercanas(double latitud, double longitud, AlTerminar<List<AdopcionCercana>> alTerminar) {
        ejecutor.execute(() -> {
            try {
                AdopcionesCercanas respuesta = ServicioUbicacionGrpc.newBlockingStub(canal.getCanal())
                        .withDeadlineAfter(LIMITE_SEGUNDOS, TimeUnit.SECONDS)
                        .obtenerAdopcionesCercanas(Coordenadas.newBuilder().setLatitud(latitud).setLongitud(longitud).build());
                List<AdopcionCercana> adopciones = new ArrayList<>();
                for (com.adoptme.movil.grpc.ubicacion.AdopcionCercana origen : respuesta.getResultadosList()) {
                    adopciones.add(convertir(origen));
                }
                alTerminar.con(Resultado.exito(adopciones));
            } catch (StatusRuntimeException excepcion) {
                alTerminar.con(Resultado.fallo(CanalGrpc.traducir(excepcion)));
            }
        });
    }

    private static AdopcionCercana convertir(com.adoptme.movil.grpc.ubicacion.AdopcionCercana origen) {
        MascotaCercana mascota = origen.getMascota();
        return new AdopcionCercana(
                origen.getAdopcionId(),
                origen.getPublicadorId(),
                origen.getLatitud(),
                origen.getLongitud(),
                new Mascota(mascota.getMascotaId(), mascota.getNombre(), mascota.getEspecie(), mascota.getRaza(),
                        mascota.getEdad(), mascota.getSexo(), mascota.getTamano(), mascota.getDescripcion()));
    }
}
