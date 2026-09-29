package com.adoptme.movil.ui.principal;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.ViewModel;

import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.dominio.Modelos.Perfil;

public final class PrincipalViewModel extends ViewModel {

    private final SesionUsuario sesion;

    public PrincipalViewModel(SesionUsuario sesion) {
        this.sesion = sesion;
    }

    public boolean haySesion() {
        return sesion.estaActiva();
    }

    public boolean esAdministrador() {
        return sesion.esAdministrador();
    }

    public LiveData<Perfil> getPerfil() {
        return sesion.perfilObservable();
    }

    public LiveData<Integer> getVersionFoto() {
        return sesion.versionFotoObservable();
    }

    public void cerrarSesion() {
        sesion.cerrar();
    }
}
