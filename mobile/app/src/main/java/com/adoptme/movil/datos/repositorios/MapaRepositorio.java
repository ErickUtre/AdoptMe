package com.adoptme.movil.datos.repositorios;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.dominio.AdopcionCercana;

import java.util.List;

public interface MapaRepositorio {

    void obtenerCercanas(double latitud, double longitud, AlTerminar<List<AdopcionCercana>> alTerminar);
}
