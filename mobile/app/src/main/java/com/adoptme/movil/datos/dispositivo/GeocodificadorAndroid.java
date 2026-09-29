package com.adoptme.movil.datos.dispositivo;

import android.content.Context;
import android.location.Address;
import android.location.Geocoder;
import android.os.Build;

import androidx.annotation.RequiresApi;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.Geocodificador;
import com.adoptme.movil.dominio.Ubicacion;

import java.io.IOException;
import java.util.List;
import java.util.Locale;
import java.util.concurrent.Executor;

public final class GeocodificadorAndroid implements Geocodificador {

    private static final int MAXIMO_RESULTADOS = 1;

    private final Geocoder geocoder;
    private final Executor ejecutor;

    public GeocodificadorAndroid(Context contexto, Executor ejecutor) {
        this.geocoder = new Geocoder(contexto.getApplicationContext(), Locale.getDefault());
        this.ejecutor = ejecutor;
    }

    @Override
    public void describir(double latitud, double longitud, AlTerminar<Ubicacion> alTerminar) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            describirAsincrono(latitud, longitud, alTerminar);
        } else {
            ejecutor.execute(() -> describirBloqueante(latitud, longitud, alTerminar));
        }
    }

    @RequiresApi(Build.VERSION_CODES.TIRAMISU)
    private void describirAsincrono(double latitud, double longitud, AlTerminar<Ubicacion> alTerminar) {
        try {
            geocoder.getFromLocation(latitud, longitud, MAXIMO_RESULTADOS, new Geocoder.GeocodeListener() {
                @Override
                public void onGeocode(List<Address> direcciones) {
                    alTerminar.con(Resultado.exito(convertir(latitud, longitud, direcciones)));
                }

                @Override
                public void onError(String mensaje) {
                    alTerminar.con(Resultado.fallo(TipoError.CONEXION, mensaje));
                }
            });
        } catch (IllegalArgumentException excepcion) {
            alTerminar.con(Resultado.fallo(TipoError.VALIDACION, excepcion.getMessage()));
        }
    }

    @SuppressWarnings("deprecation")
    private void describirBloqueante(double latitud, double longitud, AlTerminar<Ubicacion> alTerminar) {
        try {
            List<Address> direcciones = geocoder.getFromLocation(latitud, longitud, MAXIMO_RESULTADOS);
            alTerminar.con(Resultado.exito(convertir(latitud, longitud, direcciones)));
        } catch (IOException excepcion) {
            alTerminar.con(Resultado.fallo(TipoError.CONEXION, excepcion.getMessage()));
        } catch (IllegalArgumentException excepcion) {
            alTerminar.con(Resultado.fallo(TipoError.VALIDACION, excepcion.getMessage()));
        }
    }

    private static Ubicacion convertir(double latitud, double longitud, List<Address> direcciones) {
        if (direcciones == null || direcciones.isEmpty()) {
            return new Ubicacion(latitud, longitud, null, null, null);
        }
        Address direccion = direcciones.get(0);
        return new Ubicacion(latitud, longitud, direccion.getLocality(), direccion.getAdminArea(), direccion.getCountryName());
    }
}
