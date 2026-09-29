package com.adoptme.movil.datos.repositorios;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.dominio.Ubicacion;

public interface Geocodificador {

    void describir(double latitud, double longitud, AlTerminar<Ubicacion> alTerminar);
}
