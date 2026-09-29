package com.adoptme.movil.ui.reportes;

import android.graphics.Bitmap;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.datos.repositorios.Galeria;
import com.adoptme.movil.dominio.ConteoAdopciones;
import com.adoptme.movil.dominio.ConteoAdopciones.ConteoMensual;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

import java.util.Collections;
import java.util.List;

public final class ReporteViewModel extends VistaModeloBase {

    private static final String PREFIJO_ARCHIVO = "reporte_";

    private final AdopcionRepositorio adopciones;
    private final Galeria galeria;
    private final MutableLiveData<List<ConteoMensual>> conteos = new MutableLiveData<>(Collections.emptyList());

    public ReporteViewModel(AdopcionRepositorio adopciones, Galeria galeria) {
        this.adopciones = adopciones;
        this.galeria = galeria;
    }

    public LiveData<List<ConteoMensual>> getConteos() {
        return conteos;
    }

    public void cargar(EstadoAdopcion estado) {
        iniciarCarga();
        adopciones.listarParaReporte(estado, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                conteos.postValue(ConteoAdopciones.porMes(resultado.getValor()));
            } else {
                avisar(resultado.getError());
            }
        });
    }

    public void guardar(Bitmap grafica, EstadoAdopcion estado, long instante) {
        List<ConteoMensual> actuales = conteos.getValue();
        if (actuales == null || actuales.isEmpty()) {
            avisar(R.string.error_grafica_vacia);
            return;
        }
        iniciarCarga();
        galeria.guardarPng(grafica, PREFIJO_ARCHIVO + estado.getFiltro() + "_" + instante, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                avisar(R.string.imagen_guardada);
            } else {
                avisar(R.string.error_guardar_imagen);
            }
        });
    }
}
