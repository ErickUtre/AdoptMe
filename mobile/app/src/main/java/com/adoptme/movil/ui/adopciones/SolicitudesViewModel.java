package com.adoptme.movil.ui.adopciones;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.Evento;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.ui.comun.VistaModeloBase;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public final class SolicitudesViewModel extends VistaModeloBase {

    private final AdopcionRepositorio adopciones;
    private final MutableLiveData<List<Solicitud>> pendientes = new MutableLiveData<>(Collections.emptyList());
    private final MutableLiveData<Evento<Boolean>> adopcionAceptada = new MutableLiveData<>();
    private int adopcionId;

    public SolicitudesViewModel(AdopcionRepositorio adopciones) {
        this.adopciones = adopciones;
    }

    public LiveData<List<Solicitud>> getPendientes() {
        return pendientes;
    }

    public LiveData<Evento<Boolean>> getAdopcionAceptada() {
        return adopcionAceptada;
    }

    public void cargar(int adopcion) {
        adopcionId = adopcion;
        iniciarCarga();
        adopciones.listarSolicitudes(adopcionId, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                pendientes.postValue(resultado.getValor());
            } else {
                avisar(resultado.getError());
            }
        });
    }

    public void aceptar(Solicitud solicitud) {
        iniciarCarga();
        adopciones.aceptarSolicitud(adopcionId, solicitud.getSolicitudId(), resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                avisar(R.string.solicitud_aceptada);
                adopcionAceptada.postValue(new Evento<>(true));
            } else {
                avisar(resultado.getError());
            }
        });
    }

    public void rechazar(Solicitud solicitud) {
        iniciarCarga();
        adopciones.rechazarSolicitud(adopcionId, solicitud.getSolicitudId(), resultado -> {
            terminarCarga();
            if (!resultado.esExito()) {
                avisar(resultado.getError());
                return;
            }
            List<Solicitud> restantes = new ArrayList<>(pendientes.getValue() == null ? Collections.emptyList() : pendientes.getValue());
            restantes.removeIf(existente -> existente.getSolicitudId() == solicitud.getSolicitudId());
            pendientes.postValue(restantes);
            avisar(R.string.solicitud_rechazada);
        });
    }
}
