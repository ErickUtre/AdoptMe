package com.adoptme.movil.datos.sesion;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Modelos.Sesion;

public final class SesionUsuario {

    private final MutableLiveData<Perfil> perfilObservable = new MutableLiveData<>();
    private final MutableLiveData<Integer> versionFoto = new MutableLiveData<>(0);
    private volatile String token;
    private volatile boolean administrador;
    private volatile Perfil perfil;

    public void iniciar(Sesion sesion) {
        token = sesion.getToken();
        administrador = sesion.esAdmin();
        actualizarPerfil(sesion.getUsuario());
    }

    public void cerrar() {
        token = null;
        administrador = false;
        actualizarPerfil(null);
    }

    public boolean estaActiva() {
        return token != null && perfil != null;
    }

    public String getToken() {
        return token;
    }

    public boolean esAdministrador() {
        return administrador;
    }

    public Perfil getPerfil() {
        return perfil;
    }

    public int getUsuarioId() {
        return perfil == null ? 0 : perfil.getUsuarioId();
    }

    public LiveData<Perfil> perfilObservable() {
        return perfilObservable;
    }

    public LiveData<Integer> versionFotoObservable() {
        return versionFoto;
    }

    public void actualizarPerfil(Perfil nuevo) {
        perfil = nuevo;
        perfilObservable.postValue(nuevo);
    }

    public void registrarFotoNueva() {
        Integer actual = versionFoto.getValue();
        versionFoto.postValue(actual == null ? 1 : actual + 1);
    }
}
