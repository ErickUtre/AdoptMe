package com.example.adoptmemovil.utilidades;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;

import androidx.activity.result.ActivityResultCaller;
import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.core.content.ContextCompat;

import java.util.Map;

public class PermisoHelper {

    private final ActivityResultLauncher<String[]> launcher;

    public PermisoHelper(ActivityResultCaller caller, CallbackPermiso callback) {
        this.launcher = caller.registerForActivityResult(
                new ActivityResultContracts.RequestMultiplePermissions(),
                result -> {
                    boolean concedido = false;
                    for (Map.Entry<String, Boolean> entry : result.entrySet()) {
                        if (entry.getValue()) {
                            concedido = true;
                            break;
                        }
                    }

                    if (concedido) {
                        callback.onPermisoConcedido();
                    } else {
                        callback.onPermisoDenegado();
                    }
                }
        );
    }

    public void solicitarPermisoUbicacion() {
        launcher.launch(new String[]{
                Manifest.permission.ACCESS_FINE_LOCATION,
                Manifest.permission.ACCESS_COARSE_LOCATION
        });
    }

    public static boolean tienePermisoUbicacion(Context context) {
        return ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
                || ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_COARSE_LOCATION) == PackageManager.PERMISSION_GRANTED;
    }
}
