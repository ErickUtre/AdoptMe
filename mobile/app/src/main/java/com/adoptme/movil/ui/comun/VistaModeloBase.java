package com.adoptme.movil.ui.comun;

import androidx.annotation.StringRes;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.adoptme.movil.comun.ErrorOperacion;
import com.adoptme.movil.comun.Evento;

public abstract class VistaModeloBase extends ViewModel {

    private final MutableLiveData<Boolean> cargando = new MutableLiveData<>(false);
    private final MutableLiveData<Evento<Aviso>> avisos = new MutableLiveData<>();

    public LiveData<Boolean> getCargando() {
        return cargando;
    }

    public LiveData<Evento<Aviso>> getAvisos() {
        return avisos;
    }

    protected void iniciarCarga() {
        cargando.postValue(true);
    }

    protected void terminarCarga() {
        cargando.postValue(false);
    }

    protected void avisar(@StringRes int recurso, Object... argumentos) {
        avisos.postValue(new Evento<>(Aviso.de(recurso, argumentos)));
    }

    protected void avisar(ErrorOperacion error) {
        avisos.postValue(new Evento<>(Aviso.de(error)));
    }
}
