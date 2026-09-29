package com.adoptme.movil.ui.mapa;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.datos.repositorios.MapaRepositorio;
import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.dominio.AdopcionCercana;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

import java.util.Collections;
import java.util.List;

public final class MapaViewModel extends VistaModeloBase {

    static final double DESPLAZAMIENTO_MINIMO_GRADOS = 0.01;
    private static final Ubicacion CENTRO_MEXICO = new Ubicacion(23.6345, -102.5528, null, null, null);

    private final MapaRepositorio mapa;
    private final SesionUsuario sesion;
    private final MutableLiveData<List<AdopcionCercana>> adopciones = new MutableLiveData<>(Collections.emptyList());
    private double[] ultimaConsulta;

    public MapaViewModel(MapaRepositorio mapa, SesionUsuario sesion) {
        this.mapa = mapa;
        this.sesion = sesion;
    }

    public LiveData<List<AdopcionCercana>> getAdopciones() {
        return adopciones;
    }

    public Ubicacion getUbicacionUsuario() {
        return sesion.getPerfil() == null ? null : sesion.getPerfil().getUbicacion();
    }

    public Ubicacion getCentroInicial() {
        Ubicacion propia = getUbicacionUsuario();
        return propia == null ? CENTRO_MEXICO : propia;
    }

    public void cargarCercanas(double latitud, double longitud) {
        if (!sesion.estaActiva() || !seDesplazoLoSuficiente(latitud, longitud)) {
            return;
        }
        ultimaConsulta = new double[]{latitud, longitud};
        mapa.obtenerCercanas(latitud, longitud, resultado -> {
            if (resultado.esExito()) {
                adopciones.postValue(resultado.getValor());
            } else {
                avisar(resultado.getError());
            }
        });
    }

    boolean seDesplazoLoSuficiente(double latitud, double longitud) {
        return ultimaConsulta == null
                || Math.abs(latitud - ultimaConsulta[0]) >= DESPLAZAMIENTO_MINIMO_GRADOS
                || Math.abs(longitud - ultimaConsulta[1]) >= DESPLAZAMIENTO_MINIMO_GRADOS;
    }
}
