package com.adoptme.movil.ui.ubicacion;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.ActivitySeleccionUbicacionBinding;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.mapa.Marcadores;
import com.google.android.gms.location.LocationServices;

import org.osmdroid.events.MapEventsReceiver;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.overlay.MapEventsOverlay;
import org.osmdroid.views.overlay.Marker;

import java.util.Map;

public final class SeleccionUbicacionActivity extends AppCompatActivity {

    private static final double ZOOM_SELECCION = 17.0;
    private static final GeoPoint CENTRO_MEXICO = new GeoPoint(23.6345, -102.5528);

    private ActivitySeleccionUbicacionBinding vista;
    private SeleccionUbicacionViewModel vistaModelo;
    private Marker marcador;

    private final ActivityResultLauncher<String[]> solicitudPermisos = registerForActivityResult(
            new ActivityResultContracts.RequestMultiplePermissions(), this::alResolverPermisos);

    @Override
    protected void onCreate(Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        vista = ActivitySeleccionUbicacionBinding.inflate(getLayoutInflater());
        Pantallas.mostrarContenido(this, vista.getRoot());
        vistaModelo = Pantallas.vistaModelo(this, this, SeleccionUbicacionViewModel.class);
        Pantallas.observarEstado(this, vistaModelo, vista.carga.getRoot(), this);

        vista.mapa.setTileSource(TileSourceFactory.MAPNIK);
        vista.mapa.setMultiTouchControls(true);
        vista.mapa.getController().setZoom(5.0);
        vista.mapa.getController().setCenter(CENTRO_MEXICO);
        vista.mapa.getOverlays().add(new MapEventsOverlay(new MapEventsReceiver() {
            @Override
            public boolean singleTapConfirmedHelper(GeoPoint punto) {
                vistaModelo.seleccionar(punto.getLatitude(), punto.getLongitude());
                return true;
            }

            @Override
            public boolean longPressHelper(GeoPoint punto) {
                return false;
            }
        }));

        vistaModelo.getSeleccionada().observe(this, this::mostrar);
        vista.botonGuardar.setOnClickListener(boton -> guardar());
        vista.botonCancelar.setOnClickListener(boton -> finish());

        if (estadoGuardado == null) {
            iniciarUbicacion(ContratoSeleccionUbicacion.leer(getIntent()));
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        vista.mapa.onResume();
    }

    @Override
    protected void onPause() {
        vista.mapa.onPause();
        super.onPause();
    }

    private void iniciarUbicacion(Ubicacion inicial) {
        if (inicial != null) {
            vistaModelo.seleccionar(inicial.getLatitud(), inicial.getLongitud());
            return;
        }
        if (tienePermiso()) {
            obtenerUbicacionActual();
        } else {
            solicitudPermisos.launch(new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_COARSE_LOCATION});
        }
    }

    private void alResolverPermisos(Map<String, Boolean> concedidos) {
        if (concedidos.containsValue(Boolean.TRUE)) {
            obtenerUbicacionActual();
        } else {
            Toast.makeText(this, R.string.error_permiso_ubicacion, Toast.LENGTH_LONG).show();
        }
    }

    private boolean tienePermiso() {
        return ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
                || ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) == PackageManager.PERMISSION_GRANTED;
    }

    @SuppressLint("MissingPermission")
    private void obtenerUbicacionActual() {
        LocationServices.getFusedLocationProviderClient(this).getLastLocation()
                .addOnSuccessListener(this, ubicacion -> {
                    if (ubicacion == null) {
                        Toast.makeText(this, R.string.error_obtener_ubicacion, Toast.LENGTH_LONG).show();
                        return;
                    }
                    vistaModelo.seleccionar(ubicacion.getLatitude(), ubicacion.getLongitude());
                })
                .addOnFailureListener(this, error -> Toast.makeText(this, R.string.error_obtener_ubicacion, Toast.LENGTH_LONG).show());
    }

    private void mostrar(Ubicacion ubicacion) {
        GeoPoint punto = new GeoPoint(ubicacion.getLatitud(), ubicacion.getLongitud());
        if (marcador != null) {
            vista.mapa.getOverlays().remove(marcador);
        }
        marcador = Marcadores.crear(vista.mapa, punto, R.drawable.ic_ubicacion, getString(R.string.ubicacion_seleccionada));
        vista.mapa.getOverlays().add(marcador);
        vista.mapa.getController().setZoom(ZOOM_SELECCION);
        vista.mapa.getController().animateTo(punto);
        vista.mapa.invalidate();
        vista.textoDireccion.setText(Pantallas.describir(this, ubicacion));
    }

    private void guardar() {
        Ubicacion seleccionada = vistaModelo.getSeleccionada().getValue();
        if (seleccionada == null) {
            Toast.makeText(this, R.string.error_ubicacion_obligatoria, Toast.LENGTH_SHORT).show();
            return;
        }
        setResult(RESULT_OK, new Intent().putExtra(ContratoSeleccionUbicacion.EXTRA_UBICACION, seleccionada));
        finish();
    }
}
