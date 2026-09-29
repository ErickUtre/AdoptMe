package com.adoptme.movil.dominio;

public enum EstadoAdopcion {
    DISPONIBLE("disponible"),
    ADOPTADA("adoptada");

    private final String filtro;

    EstadoAdopcion(String filtro) {
        this.filtro = filtro;
    }

    public String getFiltro() {
        return filtro;
    }
}
