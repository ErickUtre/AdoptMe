package com.example.adoptmemovil.gRPC;

import static com.example.adoptmemovil.utilidades.Constantes.DIRECCION_IP;

import com.example.adoptmemovil.utilidades.HeaderClientInterceptor;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import io.grpc.Channel;
import io.grpc.ClientInterceptors;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.StatusRuntimeException;
import ubicacion.ServicioUbicacionGrpc;
import ubicacion.SolicitudCercana;
import ubicacion.SolicitudesCercanas;
import ubicacion.Ubicacion;

public class ServicioUbicacionGrpcCliente {

    private final ManagedChannel channel;
    private final ServicioUbicacionGrpc.ServicioUbicacionFutureStub stub;
    private final ExecutorService executor;

    public ServicioUbicacionGrpcCliente() {
        ManagedChannel baseChannel = ManagedChannelBuilder
                .forAddress(DIRECCION_IP, 50051)
                .usePlaintext()
                .build();

        String token = UsuarioSingleton.getInstancia().getToken();

        HeaderClientInterceptor interceptor = new HeaderClientInterceptor(token);
        Channel interceptedChannel = ClientInterceptors.intercept(baseChannel, interceptor);

        this.channel = baseChannel;
        this.stub = ServicioUbicacionGrpc.newFutureStub(interceptedChannel);

        this.executor = Executors.newSingleThreadExecutor();
    }

    public void obtenerSolicitudesCercanas(int usuarioId, double latitud, double longitud) {
        Ubicacion request = Ubicacion.newBuilder()
                .setUsuarioId(usuarioId)
                .setLatitud(latitud)
                .setLongitud(longitud)
                .build();

        com.google.common.util.concurrent.ListenableFuture<SolicitudesCercanas> future =
                stub.obtenerSolicitudesCercanas(request);

        future.addListener(() -> {
            try {
                SolicitudesCercanas response = future.get();
                for (SolicitudCercana solicitud : response.getResultadosList()) {
                    System.out.println("ID solicitud: " + solicitud.getSolicitudAdopcionId());
                    System.out.println("Distancia: " + solicitud.getDistancia());
                    System.out.println("Lat: " + solicitud.getLatitud() + ", Lon: " + solicitud.getLongitud());
                }
            } catch (ExecutionException ex) {
                Throwable causa = ex.getCause();
                if (causa instanceof StatusRuntimeException) {
                    StatusRuntimeException statusEx = (StatusRuntimeException) causa;
                    System.err.println("Error de gRPC: " + statusEx.getStatus());
                } else {
                    System.err.println("Error inesperado: " + causa);
                }
            } catch (InterruptedException ex) {
                Thread.currentThread().interrupt();
                System.err.println("La operación fue interrumpida");
            } catch (Exception ex) {
                System.err.println("Error general: " + ex);
            }
        }, executor);
    }

    public ServicioUbicacionGrpc.ServicioUbicacionFutureStub getStub() {
        return stub;
    }

    public ExecutorService getExecutor() {
        return executor;
    }

    public void shutdown() throws InterruptedException {
        channel.shutdown().awaitTermination(5, TimeUnit.SECONDS);
        executor.shutdown();
    }

}