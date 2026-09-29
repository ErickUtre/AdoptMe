package com.adoptme.movil.ui.acceso;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.Evento;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.CuentaRepositorio;
import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.ui.comun.VistaModeloBase;

public final class InicioSesionViewModel extends VistaModeloBase {

    private final CuentaRepositorio cuentas;
    private final SesionUsuario sesion;
    private final MutableLiveData<Evento<Boolean>> sesionIniciada = new MutableLiveData<>();

    public InicioSesionViewModel(CuentaRepositorio cuentas, SesionUsuario sesion) {
        this.cuentas = cuentas;
        this.sesion = sesion;
    }

    public LiveData<Evento<Boolean>> getSesionIniciada() {
        return sesionIniciada;
    }

    public void iniciarSesion(String correo, String contrasena) {
        if (correo.trim().isEmpty() || contrasena.isEmpty()) {
            avisar(R.string.error_campos_obligatorios);
            return;
        }
        iniciarCarga();
        cuentas.iniciarSesion(correo.trim(), contrasena, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                sesion.iniciar(resultado.getValor());
                sesionIniciada.postValue(new Evento<>(true));
            } else if (resultado.getError().getTipo() == TipoError.NO_AUTENTICADO) {
                avisar(R.string.credenciales_incorrectas);
            } else {
                avisar(resultado.getError());
            }
        });
    }
}
