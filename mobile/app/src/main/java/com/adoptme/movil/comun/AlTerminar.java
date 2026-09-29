package com.adoptme.movil.comun;

@FunctionalInterface
public interface AlTerminar<T> {
    void con(Resultado<T> resultado);
}
