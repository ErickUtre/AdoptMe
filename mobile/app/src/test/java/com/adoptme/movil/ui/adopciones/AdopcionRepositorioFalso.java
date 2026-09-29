package com.adoptme.movil.ui.adopciones;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.dominio.Modelos.AdopcionRegistrada;
import com.adoptme.movil.dominio.Modelos.AdopcionResumen;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.dominio.Ubicacion;

import java.util.ArrayList;
import java.util.List;

final class AdopcionRepositorioFalso implements AdopcionRepositorio {

    final List<Adopcion> propias = new ArrayList<>();
    final List<Solicitud> solicitudes = new ArrayList<>();
    final List<Integer> eliminadas = new ArrayList<>();
    final List<Integer> aceptadas = new ArrayList<>();
    final List<Integer> rechazadas = new ArrayList<>();
    Mascota mascotaRegistrada;
    boolean fallar;

    @Override
    public void registrar(Mascota mascota, Ubicacion ubicacion, AlTerminar<AdopcionRegistrada> alTerminar) {
        mascotaRegistrada = mascota;
        alTerminar.con(responder(new AdopcionRegistrada(1, 1)));
    }

    @Override
    public void listarPropias(AlTerminar<List<Adopcion>> alTerminar) {
        alTerminar.con(responder(new ArrayList<>(propias)));
    }

    @Override
    public void eliminar(int adopcionId, AlTerminar<Void> alTerminar) {
        eliminadas.add(adopcionId);
        alTerminar.con(responder(null));
    }

    @Override
    public void listarParaReporte(EstadoAdopcion estado, AlTerminar<List<AdopcionResumen>> alTerminar) {
        alTerminar.con(responder(new ArrayList<>()));
    }

    @Override
    public void listarSolicitudes(int adopcionId, AlTerminar<List<Solicitud>> alTerminar) {
        alTerminar.con(responder(new ArrayList<>(solicitudes)));
    }

    @Override
    public void solicitar(int adopcionId, AlTerminar<Void> alTerminar) {
        alTerminar.con(responder(null));
    }

    @Override
    public void aceptarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar) {
        aceptadas.add(solicitudId);
        alTerminar.con(responder(null));
    }

    @Override
    public void rechazarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar) {
        rechazadas.add(solicitudId);
        alTerminar.con(responder(null));
    }

    private <T> Resultado<T> responder(T valor) {
        return fallar ? Resultado.fallo(TipoError.CONEXION, null) : Resultado.exito(valor);
    }
}
