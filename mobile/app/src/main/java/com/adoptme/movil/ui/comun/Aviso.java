package com.adoptme.movil.ui.comun;

import android.content.Context;

import androidx.annotation.StringRes;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.ErrorOperacion;
import com.adoptme.movil.comun.TipoError;

public final class Aviso {

    private final int recurso;
    private final Object[] argumentos;
    private final ErrorOperacion error;

    private Aviso(int recurso, Object[] argumentos, ErrorOperacion error) {
        this.recurso = recurso;
        this.argumentos = argumentos;
        this.error = error;
    }

    public static Aviso de(@StringRes int recurso, Object... argumentos) {
        return new Aviso(recurso, argumentos, null);
    }

    public static Aviso de(ErrorOperacion error) {
        return new Aviso(0, new Object[0], error);
    }

    public int getRecurso() {
        return recurso;
    }

    public ErrorOperacion getError() {
        return error;
    }

    public String describir(Context contexto) {
        if (error == null) {
            return contexto.getString(recurso, argumentos);
        }
        if (error.getMensaje() != null && !error.getMensaje().isEmpty()) {
            return error.getMensaje();
        }
        return contexto.getString(recursoPara(error.getTipo()));
    }

    private static int recursoPara(TipoError tipo) {
        switch (tipo) {
            case CONEXION:
                return R.string.error_conexion;
            case NO_AUTENTICADO:
                return R.string.error_no_autenticado;
            case SERVICIO_NO_DISPONIBLE:
                return R.string.error_base_datos;
            default:
                return R.string.error_servidor;
        }
    }
}
