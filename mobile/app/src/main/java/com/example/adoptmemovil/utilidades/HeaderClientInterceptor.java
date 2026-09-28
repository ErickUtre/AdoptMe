package com.example.adoptmemovil.utilidades;

import io.grpc.*;

public class HeaderClientInterceptor implements ClientInterceptor {
    private final String tokenJwt;

    public HeaderClientInterceptor(String tokenJwt) {
        this.tokenJwt = tokenJwt;
    }

    @Override
    public <ReqT, RespT> ClientCall<ReqT, RespT> interceptCall(
            MethodDescriptor<ReqT, RespT> method,
            CallOptions callOptions,
            Channel next) {

        return new ForwardingClientCall.SimpleForwardingClientCall<>(next.newCall(method, callOptions)) {
            @Override
            public void start(Listener<RespT> responseListener, Metadata headers) {
                Metadata.Key<String> AUTHORIZATION_HEADER =
                        Metadata.Key.of("authorization", Metadata.ASCII_STRING_MARSHALLER);
                headers.put(AUTHORIZATION_HEADER, "Bearer " + tokenJwt);
                super.start(responseListener, headers);
            }
        };
    }
}
