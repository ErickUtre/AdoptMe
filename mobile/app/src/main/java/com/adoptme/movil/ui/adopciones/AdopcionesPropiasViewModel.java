package com.adoptme.movil.ui.adopciones;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public final class AdopcionesPropiasViewModel extends VistaModeloBase {

    private final AdopcionRepositorio adopciones;
    private final MutableLiveData<List<Adopcion>> lista = new MutableLiveData<>(Collections.emptyList());

    public AdopcionesPropiasViewModel(AdopcionRepositorio adopciones) {
        this.adopciones = adopciones;
    }

    public LiveData<List<Adopcion>> getLista() {
        return lista;
    }

    public void cargar() {
        iniciarCarga();
        adopciones.listarPropias(resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                lista.postValue(resultado.getValor());
            } else {
                avisar(resultado.getError());
            }
        });
    }

    public void eliminar(Adopcion adopcion) {
        iniciarCarga();
        adopciones.eliminar(adopcion.getAdopcionId(), resultado -> {
            terminarCarga();
            if (!resultado.esExito()) {
                avisar(resultado.getError());
                return;
            }
            List<Adopcion> restantes = new ArrayList<>(lista.getValue() == null ? Collections.emptyList() : lista.getValue());
            restantes.removeIf(existente -> existente.getAdopcionId() == adopcion.getAdopcionId());
            lista.postValue(restantes);
            avisar(R.string.adopcion_eliminada);
        });
    }
}
