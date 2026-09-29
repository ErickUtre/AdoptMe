package com.adoptme.movil.ui.acceso;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.Evento;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.comun.Validador;
import com.adoptme.movil.datos.repositorios.CuentaRepositorio;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

public final class RegistroUsuarioViewModel extends VistaModeloBase {

    private final CuentaRepositorio cuentas;
    private final MutableLiveData<Ubicacion> ubicacion = new MutableLiveData<>();
    private final MutableLiveData<Evento<Boolean>> registrado = new MutableLiveData<>();

    public RegistroUsuarioViewModel(CuentaRepositorio cuentas) {
        this.cuentas = cuentas;
    }

    public LiveData<Ubicacion> getUbicacion() {
        return ubicacion;
    }

    public LiveData<Evento<Boolean>> getRegistrado() {
        return registrado;
    }

    public void seleccionarUbicacion(Ubicacion seleccionada) {
        ubicacion.setValue(seleccionada);
    }

    public void registrar(String nombre, String correo, String contrasena, String confirmacion, String telefono) {
        Integer error = validar(nombre, correo, contrasena, confirmacion, telefono);
        if (error != null) {
            avisar(error);
            return;
        }
        iniciarCarga();
        cuentas.registrar(Validador.normalizar(nombre), telefono.trim(), correo.trim(), contrasena, ubicacion.getValue(), resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                avisar(R.string.registro_exitoso);
                registrado.postValue(new Evento<>(true));
            } else if (resultado.getError().getTipo() == TipoError.CONFLICTO) {
                avisar(R.string.correo_registrado);
            } else {
                avisar(resultado.getError());
            }
        });
    }

    static Integer validar(String nombre, String correo, String contrasena, String confirmacion, String telefono) {
        if (!Validador.esNombreValido(nombre)) {
            return R.string.error_nombre;
        }
        if (!Validador.esCorreoValido(correo)) {
            return R.string.error_correo;
        }
        if (!Validador.esContrasenaValida(contrasena)) {
            return R.string.error_contrasena;
        }
        if (!contrasena.equals(confirmacion)) {
            return R.string.error_confirmacion;
        }
        if (!Validador.esTelefonoValido(telefono)) {
            return R.string.error_telefono;
        }
        return null;
    }
}
