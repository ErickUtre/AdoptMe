package com.adoptme.movil.datos.remoto.grpc;

import com.adoptme.movil.comun.ErrorOperacion;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.sesion.SesionUsuario;

import io.grpc.CallOptions;
import io.grpc.Channel;
import io.grpc.ClientCall;
import io.grpc.ClientInterceptor;
import io.grpc.ClientInterceptors;
import io.grpc.ForwardingClientCall;
import io.grpc.ManagedChannel;
import io.grpc.Metadata;
import io.grpc.MethodDescriptor;
import io.grpc.Status;
import io.grpc.okhttp.OkHttpChannelBuilder;

public final class CanalGrpc {

    private static final Metadata.Key<String> AUTORIZACION = Metadata.Key.of("authorization", Metadata.ASCII_STRING_MARSHALLER);

    private final Channel canal;

    public CanalGrpc(String host, int puerto, SesionUsuario sesion) {
        ManagedChannel canalBase = OkHttpChannelBuilder.forAddress(host, puerto).usePlaintext().build();
        canal = ClientInterceptors.intercept(canalBase, new InterceptorAutenticacion(sesion));
    }

    public Channel getCanal() {
        return canal;
    }

    static ErrorOperacion traducir(Throwable causa) {
        Status estado = Status.fromThrowable(causa);
        String detalle = estado.getDescription();
        switch (estado.getCode()) {
            case INVALID_ARGUMENT:
                return new ErrorOperacion(TipoError.VALIDACION, detalle);
            case UNAUTHENTICATED:
                return new ErrorOperacion(TipoError.NO_AUTENTICADO, detalle);
            case PERMISSION_DENIED:
                return new ErrorOperacion(TipoError.PROHIBIDO, detalle);
            case NOT_FOUND:
                return new ErrorOperacion(TipoError.NO_ENCONTRADO, detalle);
            case ALREADY_EXISTS:
                return new ErrorOperacion(TipoError.CONFLICTO, detalle);
            case UNAVAILABLE:
            case DEADLINE_EXCEEDED:
                return new ErrorOperacion(TipoError.CONEXION, null);
            default:
                return new ErrorOperacion(TipoError.DESCONOCIDO, null);
        }
    }

    private static final class InterceptorAutenticacion implements ClientInterceptor {

        private final SesionUsuario sesion;

        InterceptorAutenticacion(SesionUsuario sesion) {
            this.sesion = sesion;
        }

        @Override
        public <S, R> ClientCall<S, R> interceptCall(MethodDescriptor<S, R> metodo, CallOptions opciones, Channel siguiente) {
            return new ForwardingClientCall.SimpleForwardingClientCall<>(siguiente.newCall(metodo, opciones)) {
                @Override
                public void start(Listener<R> oyente, Metadata encabezados) {
                    String token = sesion.getToken();
                    if (token != null) {
                        encabezados.put(AUTORIZACION, "Bearer " + token);
                    }
                    super.start(oyente, encabezados);
                }
            };
        }
    }
}
