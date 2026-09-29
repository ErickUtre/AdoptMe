package com.adoptme.movil.ui.perfil;

import android.net.Uri;

import androidx.lifecycle.LiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.ErrorOperacion;
import com.adoptme.movil.comun.Validador;
import com.adoptme.movil.datos.repositorios.ArchivoRepositorio;
import com.adoptme.movil.datos.repositorios.CuentaRepositorio;
import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

import java.util.Locale;

public final class PerfilViewModel extends VistaModeloBase {

    private final CuentaRepositorio cuentas;
    private final ArchivoRepositorio archivos;
    private final SesionUsuario sesion;

    public PerfilViewModel(CuentaRepositorio cuentas, ArchivoRepositorio archivos, SesionUsuario sesion) {
        this.cuentas = cuentas;
        this.archivos = archivos;
        this.sesion = sesion;
    }

    public LiveData<Perfil> getPerfil() {
        return sesion.perfilObservable();
    }

    public LiveData<Integer> getVersionFoto() {
        return sesion.versionFotoObservable();
    }

    public Integer validarNombre(String nombre) {
        if (!Validador.esNombreValido(nombre)) {
            return R.string.error_nombre;
        }
        return Validador.normalizar(nombre).equals(actual().getNombre()) ? R.string.error_mismo_valor : null;
    }

    public Integer validarCorreo(String correo) {
        if (!Validador.esCorreoValido(correo)) {
            return R.string.error_correo;
        }
        return correo.trim().equalsIgnoreCase(actual().getAcceso().getCorreo()) ? R.string.error_mismo_valor : null;
    }

    public Integer validarTelefono(String telefono) {
        if (!Validador.esTelefonoValido(telefono)) {
            return R.string.error_telefono;
        }
        return telefono.trim().equals(actual().getTelefono()) ? R.string.error_mismo_valor : null;
    }

    public void actualizarNombre(String nombre) {
        iniciarCarga();
        cuentas.actualizarNombre(Validador.normalizar(nombre), resultado -> {
            terminarCarga();
            publicarPerfil(resultado.esExito() ? resultado.getValor() : null, resultado.getError());
        });
    }

    public void actualizarTelefono(String telefono) {
        iniciarCarga();
        cuentas.actualizarTelefono(telefono.trim(), resultado -> {
            terminarCarga();
            publicarPerfil(resultado.esExito() ? resultado.getValor() : null, resultado.getError());
        });
    }

    public void actualizarCorreo(String correo) {
        String normalizado = correo.trim().toLowerCase(Locale.ROOT);
        iniciarCarga();
        cuentas.actualizarCorreo(normalizado, resultado -> {
            terminarCarga();
            publicarPerfil(resultado.esExito() ? actual().conCorreo(normalizado) : null, resultado.getError());
        });
    }

    public void actualizarUbicacion(Ubicacion ubicacion) {
        iniciarCarga();
        cuentas.actualizarUbicacion(ubicacion, resultado -> {
            terminarCarga();
            publicarPerfil(resultado.esExito() ? actual().conUbicacion(resultado.getValor()) : null, resultado.getError());
        });
    }

    public void cambiarFoto(Uri archivo) {
        iniciarCarga();
        archivos.subirFotoPerfil(archivo, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                sesion.registrarFotoNueva();
                avisar(R.string.foto_actualizada);
            } else {
                avisar(resultado.getError());
            }
        });
    }

    private Perfil actual() {
        return sesion.getPerfil();
    }

    private void publicarPerfil(Perfil actualizado, ErrorOperacion error) {
        if (actualizado == null) {
            avisar(error);
            return;
        }
        sesion.actualizarPerfil(actualizado);
        avisar(R.string.perfil_actualizado);
    }
}
