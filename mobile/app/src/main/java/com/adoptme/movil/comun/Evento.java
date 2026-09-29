package com.adoptme.movil.comun;

public final class Evento<T> {

    private final T contenido;
    private boolean manejado;

    public Evento(T contenido) {
        this.contenido = contenido;
    }

    public synchronized T consumir() {
        if (manejado) {
            return null;
        }
        manejado = true;
        return contenido;
    }
}
