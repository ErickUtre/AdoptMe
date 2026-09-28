package com.example.adoptmemovil;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.drawable.Drawable;
import android.location.Address;
import android.location.Geocoder;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.example.adoptmemovil.modelo.Ubicacion;
import com.example.adoptmemovil.utilidades.*;

import org.osmdroid.config.Configuration;
import org.osmdroid.events.MapEventsReceiver;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.MapView;
import org.osmdroid.views.overlay.MapEventsOverlay;
import org.osmdroid.views.overlay.Marker;

import java.io.Console;
import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.Locale;

public class MapaRegistroActivity extends AppCompatActivity {

    private MapView mapView;
    private TextView tvDatosUbicacion;
    private Ubicacion ubicacion;
    private Marker ubicacionActual;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_mapa_registro);

        File osmDir = new File(getFilesDir(), "osmdroid");
        File tileCache = new File(osmDir, "tiles");
        Configuration.getInstance().setOsmdroidBasePath(osmDir);
        Configuration.getInstance().setOsmdroidTileCache(tileCache);

        Configuration.getInstance().load(
                getApplicationContext(),
                getSharedPreferences("osmdroid", 0)
        );

        mapView = findViewById(R.id.mapaRegistro);
        mapView.setTileSource(TileSourceFactory.MAPNIK);
        mapView.setMultiTouchControls(true);

        // Obtener datos del intent
        double latitud = getIntent().getDoubleExtra("latitud", 0.0);
        double longitud = getIntent().getDoubleExtra("longitud", 0.0);
        String ciudad = getIntent().getStringExtra("ciudad");
        String estado = getIntent().getStringExtra("estado");
        String pais = getIntent().getStringExtra("pais");

        ubicacion = new Ubicacion();
        ubicacion.setLatitud(latitud);
        ubicacion.setLongitud(longitud);
        ubicacion.setCiudad(ciudad);
        ubicacion.setEstado(estado);
        ubicacion.setPais(pais);

        tvDatosUbicacion = findViewById(R.id.tvDatosUbicacion);
        tvDatosUbicacion.setText(ubicacion.toString());

        inicializarMapaConUbicacion(ubicacion);

        MapEventsReceiver mapEventsReceiver = new MapEventsReceiver() {
            @Override
            public boolean singleTapConfirmedHelper(GeoPoint punto) {
                establecerUbicacionSeleccionada(punto);
                return true;
            }

            @Override
            public boolean longPressHelper(GeoPoint punto) {
                return false;
            }
        };

        MapEventsOverlay mapEventsOverlay = new MapEventsOverlay(mapEventsReceiver);
        mapView.getOverlays().add(mapEventsOverlay);


        Button btnGuardar = findViewById(R.id.btnGuardar);
        btnGuardar.setOnClickListener(view -> {
            Intent data = new Intent();
            data.putExtra("latitud", ubicacion.getLatitud());
            data.putExtra("longitud", ubicacion.getLongitud());
            data.putExtra("ciudad", ubicacion.getCiudad());
            data.putExtra("estado", ubicacion.getEstado());
            data.putExtra("pais", ubicacion.getPais());
            setResult(RESULT_OK, data);
            finish();
        });

        Button btnCancelar = findViewById(R.id.btnCancelar);
        btnCancelar.setOnClickListener(view -> {
            setResult(RESULT_CANCELED);
            finish();
        });
    }

    private void inicializarMapaConUbicacion(Ubicacion ubicacion) {
        GeoPoint punto = new GeoPoint(ubicacion.getLatitud(), ubicacion.getLongitud());
        mapView.getController().setZoom(19.0);
        mapView.getController().setCenter(punto);

        ubicacionActual = new Marker(mapView);
        ubicacionActual.setPosition(punto);
        ubicacionActual.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);

        Drawable original = ContextCompat.getDrawable(this, R.drawable.ic_ubicacion);
        Drawable escalado = InterfazUsuarioUtils.escalarDrawable(this, original, 0.5f);
        ubicacionActual.setIcon(escalado);

        ubicacionActual.setTitle("Tu ubicación");

        mapView.getOverlays().add(ubicacionActual);

    }

    private void establecerUbicacionSeleccionada(GeoPoint punto) {
        if (ubicacionActual != null) {
            mapView.getOverlays().remove(ubicacionActual);
        }

        ubicacionActual = new Marker(mapView);
        ubicacionActual.setPosition(punto);
        ubicacionActual.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);

        Drawable icono = ContextCompat.getDrawable(this, R.drawable.ic_ubicacion);
        ubicacionActual.setIcon(InterfazUsuarioUtils.escalarDrawable(this, icono, 0.5f));

        mapView.getOverlays().add(ubicacionActual);
        mapView.invalidate();

        guardarUbicacionTemporalmenteYMostrar(punto);
    }

    private void guardarUbicacionTemporalmenteYMostrar(GeoPoint punto) {
        Geocoder geocoder = new Geocoder(this, Locale.getDefault());
        try {
            List<Address> direcciones = geocoder.getFromLocation(punto.getLatitude(), punto.getLongitude(), 1);
            assert direcciones != null;
            if (!direcciones.isEmpty()) {
                Address direccion = direcciones.get(0);
                String ciudad = direccion.getLocality();
                String estado = direccion.getAdminArea();
                String pais = direccion.getCountryName();

                ubicacion.setLatitud(punto.getLatitude());
                ubicacion.setLongitud(punto.getLongitude());
                ubicacion.setCiudad(ciudad);
                ubicacion.setEstado(estado);
                ubicacion.setPais(pais);

                tvDatosUbicacion.setText(ubicacion.toString());
            }
        } catch (IOException e) {
            Toast.makeText(this, "No se pudo obtener dirección", Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        mapView.onResume();
    }

    @Override
    protected void onPause() {
        super.onPause();
        mapView.onPause();
    }
}
