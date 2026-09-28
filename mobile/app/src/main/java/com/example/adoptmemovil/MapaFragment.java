package com.example.adoptmemovil;

import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import org.osmdroid.config.Configuration;
import org.osmdroid.events.DelayedMapListener;
import org.osmdroid.events.MapListener;
import org.osmdroid.events.ScrollEvent;
import org.osmdroid.events.ZoomEvent;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.MapView;
import org.osmdroid.views.overlay.Marker;
import org.osmdroid.views.overlay.ScaleBarOverlay;
import org.osmdroid.views.overlay.compass.CompassOverlay;
import org.osmdroid.views.overlay.gestures.RotationGestureOverlay;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import com.example.adoptmemovil.gRPC.ServicioUbicacionGrpcCliente;
import com.example.adoptmemovil.modelo.Ubicacion;
import com.example.adoptmemovil.utilidades.InterfazUsuarioUtils;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

import ubicacion.SolicitudCercana;
import ubicacion.SolicitudesCercanas;
public class MapaFragment extends Fragment {

    private MapView mapView;
    private List<Marker> marcadoresSolicitudes = new ArrayList<>();
    private Marker marcadorUsuario = null;
    private ServicioUbicacionGrpcCliente servicioGrpc;

    private final Handler debounceHandler = new Handler(Looper.getMainLooper());
    private Runnable debounceRunnable;

    private static final double ZOOM_VISIBLE = 18.0;
    private static final GeoPoint POSICION_DEFECTO_MEXICO = new GeoPoint(23.6345, -102.5528);

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View root = inflater.inflate(R.layout.fragment_mapa, container, false);

        configurarMapView(root);
        servicioGrpc = new ServicioUbicacionGrpcCliente();

        Ubicacion ubicacionUsuario = null;
        if (UsuarioSingleton.getInstancia().estaLogueado()) {
            ubicacionUsuario = UsuarioSingleton.getInstancia().getUsuarioActual().getUbicacion();
        }

        if (ubicacionUsuario != null) {
            mostrarUbicacionUsuario(ubicacionUsuario);
        }

        return root;
    }

    private void configurarMapView(View root) {
        File osmDir = new File(requireContext().getFilesDir(), "osmdroid");
        Configuration.getInstance().setOsmdroidBasePath(osmDir);
        Configuration.getInstance().setOsmdroidTileCache(new File(osmDir, "tiles"));
        Configuration.getInstance().load(requireContext(), requireContext().getSharedPreferences("osmdroid", 0));

        mapView = root.findViewById(R.id.mapaPrincipal);
        mapView.setTileSource(TileSourceFactory.MAPNIK);
        mapView.setMultiTouchControls(true);
        mapView.getController().setZoom(6.0);
        mapView.getController().setCenter(POSICION_DEFECTO_MEXICO);

        mapView.getOverlays().add(new CompassOverlay(requireContext(), mapView));
        mapView.getOverlays().add(new RotationGestureOverlay(mapView));
        mapView.getOverlays().add(new ScaleBarOverlay(mapView));

        mapView.addMapListener(new DelayedMapListener(new MapListener() {
            @Override
            public boolean onScroll(ScrollEvent e) {
                if (debounceRunnable != null) debounceHandler.removeCallbacks(debounceRunnable);
                debounceRunnable = () -> {
                    GeoPoint centro = (GeoPoint) mapView.getMapCenter();
                    obtenerUsuariosCercanos(centro.getLatitude(), centro.getLongitude());
                };
                debounceHandler.postDelayed(debounceRunnable, 1000);
                return true;
            }

            @Override
            public boolean onZoom(ZoomEvent e) {
                boolean visible = mapView.getZoomLevelDouble() >= ZOOM_VISIBLE;
                for (Marker m : marcadoresSolicitudes)
                    m.setEnabled(visible);
                return true;
            }
        }, 200)); // 200ms debounce


        mapView.addMapListener(new DelayedMapListener(new MapListener() {
            @Override public boolean onZoom(ZoomEvent e) {
                boolean visible = mapView.getZoomLevelDouble() >= ZOOM_VISIBLE;
                for (Marker m : marcadoresSolicitudes)
                    m.setEnabled(visible);
                return true;
            }
            @Override public boolean onScroll(ScrollEvent e) { return false; }
        }, 200));
    }

    private void mostrarUbicacionUsuario(Ubicacion ubicacionUsuario) {
        GeoPoint geo = new GeoPoint(ubicacionUsuario.getLatitud(), ubicacionUsuario.getLongitud());
        agregarMarcadorUsuario(geo);
        mapView.getController().setZoom(17.0);
        mapView.getController().setCenter(geo);
        obtenerUsuariosCercanos(geo.getLatitude(), geo.getLongitude());
    }

    private void agregarMarcadorUsuario(GeoPoint ubicacion) {
        if (marcadorUsuario != null) mapView.getOverlays().remove(marcadorUsuario);

        Marker marker = new Marker(mapView);
        marker.setPosition(ubicacion);
        marker.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);
        Drawable icono = InterfazUsuarioUtils.escalarDrawable(requireContext(),
                ContextCompat.getDrawable(requireContext(), R.drawable.ic_ubicacion), 0.5f);
        marker.setIcon(icono);
        marker.setTitle("Tu ubicación");

        mapView.getOverlays().add(marker);
        marcadorUsuario = marker;
    }

    private void agregarMarcadorSolicitud(GeoPoint ubicacion, ubicacion.SolicitudCercana solicitudCercana) {
        Marker marker = new Marker(mapView);
        marker.setPosition(ubicacion);
        marker.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);

        Drawable icono = InterfazUsuarioUtils.escalarDrawable(requireContext(),
                ContextCompat.getDrawable(requireContext(), R.drawable.ic_adopcion), 0.5f);
        marker.setIcon(icono);

        marker.setOnMarkerClickListener((Marker m, MapView mapView) -> {
            com.example.adoptmemovil.modelo.Mascota mascota = new com.example.adoptmemovil.modelo.Mascota();

            ubicacion.Mascota grpcMascota = solicitudCercana.getMascota();

            mascota.setMascotaID(grpcMascota.getMascotaId());
            mascota.setNombre(grpcMascota.getNombre());
            mascota.setEspecie(grpcMascota.getEspecie());
            mascota.setRaza(grpcMascota.getRaza());
            mascota.setEdad(grpcMascota.getEdad());
            mascota.setSexo(grpcMascota.getSexo());
            mascota.setTamaño(grpcMascota.getTamano());
            mascota.setDescripcion(grpcMascota.getDescripcion());

            MascotaBottomSheet bottomSheet = MascotaBottomSheet.newInstance(mascota);
            bottomSheet.show(getParentFragmentManager(), bottomSheet.getTag());
            return true;
        });

        mapView.getOverlays().add(marker);
        marcadoresSolicitudes.add(marker);
    }

    private void limpiarMarcadoresSolicitudes() {
        for (Marker marcador : marcadoresSolicitudes)
            mapView.getOverlays().remove(marcador);
        marcadoresSolicitudes.clear();
    }

    private void obtenerUsuariosCercanos(double latitud, double longitud) {
        if (servicioGrpc == null) return;

        if (!UsuarioSingleton.getInstancia().estaLogueado()) {
            Log.w("MapaFragment", "Usuario no logueado, se cancela obtenerUsuariosCercanos");
            return;
        }

        ubicacion.Ubicacion request = ubicacion.Ubicacion.newBuilder()
                .setUsuarioId(UsuarioSingleton.getInstancia().getUsuarioActual().getUsuarioID())
                .setLatitud(latitud)
                .setLongitud(longitud)
                .build();

        servicioGrpc.getStub().obtenerSolicitudesCercanas(request).addListener(() -> {
            try {
                SolicitudesCercanas respuesta = servicioGrpc.getStub().obtenerSolicitudesCercanas(request).get();

                requireActivity().runOnUiThread(() -> {
                    limpiarMarcadoresSolicitudes();
                    for (SolicitudCercana sc : respuesta.getResultadosList()) {
                        GeoPoint punto = new GeoPoint(sc.getLatitud(), sc.getLongitud());
                        agregarMarcadorSolicitud(punto, sc);
                    }
                    mapView.invalidate();
                });

            } catch (Exception ex) {
                requireActivity().runOnUiThread(() ->
                        Toast.makeText(requireContext(), "Error: " + ex.getMessage(), Toast.LENGTH_SHORT).show());
                Log.e("gRPC", "Error obteniendo solicitudes", ex);
            }
        }, servicioGrpc.getExecutor());
    }

    @Override public void onResume() { super.onResume(); mapView.onResume(); }
    @Override public void onPause() { super.onPause(); mapView.onPause(); }
    @Override public void onDestroy() {
        super.onDestroy();
        if (servicioGrpc != null) {
            try { servicioGrpc.shutdown(); } catch (InterruptedException e) { e.printStackTrace(); }
        }
    }
}