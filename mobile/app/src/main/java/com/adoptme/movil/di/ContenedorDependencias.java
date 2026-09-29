package com.adoptme.movil.di;

import android.content.Context;

import androidx.lifecycle.ViewModelProvider;

import com.adoptme.movil.BuildConfig;
import com.adoptme.movil.datos.dispositivo.GaleriaAndroid;
import com.adoptme.movil.datos.dispositivo.GeocodificadorAndroid;
import com.adoptme.movil.datos.remoto.AdopcionRepositorioRest;
import com.adoptme.movil.datos.remoto.ClienteRest;
import com.adoptme.movil.datos.remoto.CuentaRepositorioRest;
import com.adoptme.movil.datos.remoto.grpc.CanalGrpc;
import com.adoptme.movil.datos.remoto.grpc.FuenteArchivosGrpc;
import com.adoptme.movil.datos.remoto.grpc.FuenteMapaGrpc;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.datos.repositorios.ArchivoRepositorio;
import com.adoptme.movil.datos.repositorios.CuentaRepositorio;
import com.adoptme.movil.datos.repositorios.Galeria;
import com.adoptme.movil.datos.repositorios.Geocodificador;
import com.adoptme.movil.datos.repositorios.MapaRepositorio;
import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.ui.comun.RecursosRemotos;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public final class ContenedorDependencias {

    private static final int HILOS_TRABAJO = 4;

    private final SesionUsuario sesion = new SesionUsuario();
    private final CuentaRepositorio cuentas;
    private final AdopcionRepositorio adopciones;
    private final MapaRepositorio mapa;
    private final ArchivoRepositorio archivos;
    private final Geocodificador geocodificador;
    private final Galeria galeria;
    private final RecursosRemotos recursosRemotos;
    private final ViewModelProvider.Factory fabricaViewModels;

    public ContenedorDependencias(Context contexto) {
        ClienteRest clienteRest = new ClienteRest(BuildConfig.URL_API, sesion);
        Executor ejecutor = Executors.newFixedThreadPool(HILOS_TRABAJO);
        CanalGrpc canalGrpc = new CanalGrpc(BuildConfig.HOST_GRPC, BuildConfig.PUERTO_GRPC, sesion);
        cuentas = new CuentaRepositorioRest(clienteRest);
        adopciones = new AdopcionRepositorioRest(clienteRest);
        mapa = new FuenteMapaGrpc(canalGrpc, ejecutor);
        archivos = new FuenteArchivosGrpc(canalGrpc, contexto.getContentResolver(), ejecutor);
        geocodificador = new GeocodificadorAndroid(contexto, ejecutor);
        galeria = new GaleriaAndroid(contexto, ejecutor);
        recursosRemotos = new RecursosRemotos(clienteRest.getUrlBase(), sesion);
        fabricaViewModels = new FabricaViewModels(this);
    }

    public SesionUsuario getSesion() {
        return sesion;
    }

    public CuentaRepositorio getCuentas() {
        return cuentas;
    }

    public AdopcionRepositorio getAdopciones() {
        return adopciones;
    }

    public MapaRepositorio getMapa() {
        return mapa;
    }

    public ArchivoRepositorio getArchivos() {
        return archivos;
    }

    public Geocodificador getGeocodificador() {
        return geocodificador;
    }

    public Galeria getGaleria() {
        return galeria;
    }

    public RecursosRemotos getRecursosRemotos() {
        return recursosRemotos;
    }

    public ViewModelProvider.Factory getFabricaViewModels() {
        return fabricaViewModels;
    }
}
