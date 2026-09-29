package com.adoptme.movil.comun;

import java.util.Objects;

public final class Resultado<T> {

    private final T valor;
    private final ErrorOperacion error;

    private Resultado(T valor, ErrorOperacion error) {
        this.valor = valor;
        this.error = error;
    }

    public static <T> Resultado<T> exito(T valor) {
        return new Resultado<>(valor, null);
    }

    public static <T> Resultado<T> fallo(ErrorOperacion error) {
        return new Resultado<>(null, Objects.requireNonNull(error));
    }

    public static <T> Resultado<T> fallo(TipoError tipo, String mensaje) {
        return fallo(new ErrorOperacion(tipo, mensaje));
    }

    public boolean esExito() {
        return error == null;
    }

    public T getValor() {
        return valor;
    }

    public ErrorOperacion getError() {
        return error;
    }

    public <R> Resultado<R> sinValor() {
        return esExito() ? exito(null) : fallo(error);
    }
}
