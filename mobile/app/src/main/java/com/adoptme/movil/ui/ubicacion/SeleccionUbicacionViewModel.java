package com.adoptme.movil.ui.ubicacion;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.datos.repositorios.Geocodificador;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

public final class SeleccionUbicacionViewModel extends VistaModeloBase {

    private final Geocodificador geocodificador;
    private final MutableLiveData<Ubicacion> seleccionada = new MutableLiveData<>();

    public SeleccionUbicacionViewModel(Geocodificador geocodificador) {
        this.geocodificador = geocodificador;
    }

    public LiveData<Ubicacion> getSeleccionada() {
        return seleccionada;
    }

    public void seleccionar(double latitud, double longitud) {
        iniciarCarga();
        geocodificador.describir(latitud, longitud, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                seleccionada.postValue(resultado.getValor());
            } else {
                seleccionada.postValue(new Ubicacion(latitud, longitud, null, null, null));
                avisar(R.string.error_direccion);
            }
        });
    }
}
