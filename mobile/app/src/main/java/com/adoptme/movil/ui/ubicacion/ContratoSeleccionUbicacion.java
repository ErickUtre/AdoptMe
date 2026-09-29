package com.adoptme.movil.ui.ubicacion;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;

import androidx.activity.result.contract.ActivityResultContract;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.IntentCompat;

import com.adoptme.movil.dominio.Ubicacion;

public final class ContratoSeleccionUbicacion extends ActivityResultContract<Ubicacion, Ubicacion> {

    static final String EXTRA_UBICACION = "com.adoptme.movil.extra.UBICACION";

    static Ubicacion leer(Intent intencion) {
        return intencion == null ? null : IntentCompat.getParcelableExtra(intencion, EXTRA_UBICACION, Ubicacion.class);
    }

    @NonNull
    @Override
    public Intent createIntent(@NonNull Context contexto, @Nullable Ubicacion inicial) {
        Intent intencion = new Intent(contexto, SeleccionUbicacionActivity.class);
        if (inicial != null) {
            intencion.putExtra(EXTRA_UBICACION, inicial);
        }
        return intencion;
    }

    @Override
    public Ubicacion parseResult(int codigo, @Nullable Intent datos) {
        return codigo == Activity.RESULT_OK ? leer(datos) : null;
    }
}
