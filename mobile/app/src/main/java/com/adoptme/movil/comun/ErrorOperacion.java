package com.adoptme.movil.comun;

public final class ErrorOperacion {

    private final TipoError tipo;
    private final String mensaje;

    public ErrorOperacion(TipoError tipo, String mensaje) {
        this.tipo = tipo;
        this.mensaje = mensaje;
    }

    public TipoError getTipo() {
        return tipo;
    }

    public String getMensaje() {
        return mensaje;
    }
}
