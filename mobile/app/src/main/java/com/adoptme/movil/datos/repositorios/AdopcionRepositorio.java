package com.adoptme.movil.datos.repositorios;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.dominio.Modelos.AdopcionRegistrada;
import com.adoptme.movil.dominio.Modelos.AdopcionResumen;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.dominio.Ubicacion;

import java.util.List;

public interface AdopcionRepositorio {

    void registrar(Mascota mascota, Ubicacion ubicacion, AlTerminar<AdopcionRegistrada> alTerminar);

    void listarPropias(AlTerminar<List<Adopcion>> alTerminar);

    void eliminar(int adopcionId, AlTerminar<Void> alTerminar);

    void listarParaReporte(EstadoAdopcion estado, AlTerminar<List<AdopcionResumen>> alTerminar);

    void listarSolicitudes(int adopcionId, AlTerminar<List<Solicitud>> alTerminar);

    void solicitar(int adopcionId, AlTerminar<Void> alTerminar);

    void aceptarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar);

    void rechazarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar);
}
