package com.adoptme.movil.ui.mapa;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.FragmentMapaBinding;
import com.adoptme.movil.dominio.AdopcionCercana;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.Aviso;
import com.adoptme.movil.ui.comun.Pantallas;

import org.osmdroid.events.MapListener;
import org.osmdroid.events.ScrollEvent;
import org.osmdroid.events.ZoomEvent;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.overlay.Marker;

import java.util.ArrayList;
import java.util.List;

public final class MapaFragment extends Fragment {

    private static final double ZOOM_DETALLE = 16.0;
    private static final double ZOOM_PAIS = 5.0;
    private static final double ZOOM_MINIMO_MARCADORES = 12.0;
    private static final long ESPERA_DESPLAZAMIENTO_MS = 800;

    private final Handler manejador = new Handler(Looper.getMainLooper());
    private final List<Marker> marcadoresAdopciones = new ArrayList<>();
    private final Runnable consultarCentro = this::consultarCentro;
    private FragmentMapaBinding vista;
    private MapaViewModel vistaModelo;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentMapaBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        vistaModelo = Pantallas.vistaModelo(this, requireContext(), MapaViewModel.class);
        vistaModelo.getAvisos().observe(getViewLifecycleOwner(), evento -> {
            Aviso aviso = evento.consumir();
            if (aviso != null) {
                Toast.makeText(requireContext(), aviso.describir(requireContext()), Toast.LENGTH_SHORT).show();
            }
        });
        configurarMapa();
        vistaModelo.getAdopciones().observe(getViewLifecycleOwner(), this::dibujarAdopciones);
    }

    @Override
    public void onResume() {
        super.onResume();
        vista.mapa.onResume();
    }

    @Override
    public void onPause() {
        vista.mapa.onPause();
        super.onPause();
    }

    @Override
    public void onDestroyView() {
        manejador.removeCallbacks(consultarCentro);
        marcadoresAdopciones.clear();
        vista = null;
        super.onDestroyView();
    }

    private void configurarMapa() {
        vista.mapa.setTileSource(TileSourceFactory.MAPNIK);
        vista.mapa.setMultiTouchControls(true);

        Ubicacion propia = vistaModelo.getUbicacionUsuario();
        Ubicacion centro = vistaModelo.getCentroInicial();
        GeoPoint punto = new GeoPoint(centro.getLatitud(), centro.getLongitud());
        vista.mapa.getController().setZoom(propia == null ? ZOOM_PAIS : ZOOM_DETALLE);
        vista.mapa.getController().setCenter(punto);
        if (propia != null) {
            vista.mapa.getOverlays().add(Marcadores.crear(vista.mapa, punto, R.drawable.ic_ubicacion, getString(R.string.tu_ubicacion)));
        }

        vista.mapa.addMapListener(new MapListener() {
            @Override
            public boolean onScroll(ScrollEvent evento) {
                programarConsulta();
                return false;
            }

            @Override
            public boolean onZoom(ZoomEvent evento) {
                actualizarVisibilidad();
                programarConsulta();
                return false;
            }
        });
        vistaModelo.cargarCercanas(centro.getLatitud(), centro.getLongitud());
    }

    private void programarConsulta() {
        manejador.removeCallbacks(consultarCentro);
        manejador.postDelayed(consultarCentro, ESPERA_DESPLAZAMIENTO_MS);
    }

    private void consultarCentro() {
        if (vista == null) {
            return;
        }
        GeoPoint centro = (GeoPoint) vista.mapa.getMapCenter();
        vistaModelo.cargarCercanas(centro.getLatitude(), centro.getLongitude());
    }

    private void dibujarAdopciones(List<AdopcionCercana> adopciones) {
        vista.mapa.getOverlays().removeAll(marcadoresAdopciones);
        marcadoresAdopciones.clear();
        for (AdopcionCercana adopcion : adopciones) {
            Marker marcador = Marcadores.crear(vista.mapa, new GeoPoint(adopcion.getLatitud(), adopcion.getLongitud()),
                    R.drawable.ic_adopcion, adopcion.getMascota().getNombre());
            marcador.setOnMarkerClickListener((marcadorTocado, mapa) -> {
                HojaMascota.nueva(adopcion).show(getChildFragmentManager(), HojaMascota.ETIQUETA);
                return true;
            });
            marcadoresAdopciones.add(marcador);
        }
        vista.mapa.getOverlays().addAll(marcadoresAdopciones);
        actualizarVisibilidad();
        vista.mapa.invalidate();
    }

    private void actualizarVisibilidad() {
        boolean visibles = vista.mapa.getZoomLevelDouble() >= ZOOM_MINIMO_MARCADORES;
        for (Marker marcador : marcadoresAdopciones) {
            marcador.setVisible(visibles);
        }
    }
}
