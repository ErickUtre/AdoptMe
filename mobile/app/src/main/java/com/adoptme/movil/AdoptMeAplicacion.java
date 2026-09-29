package com.adoptme.movil;

import android.app.Application;
import android.content.Context;

import com.adoptme.movil.di.ContenedorDependencias;

import org.osmdroid.config.Configuration;
import org.osmdroid.config.IConfigurationProvider;

import java.io.File;

public final class AdoptMeAplicacion extends Application {

    private ContenedorDependencias contenedor;

    public static ContenedorDependencias contenedor(Context contexto) {
        return ((AdoptMeAplicacion) contexto.getApplicationContext()).contenedor;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        contenedor = new ContenedorDependencias(this);
        configurarMapas();
    }

    private void configurarMapas() {
        IConfigurationProvider configuracion = Configuration.getInstance();
        File directorio = new File(getFilesDir(), "osmdroid");
        configuracion.setOsmdroidBasePath(directorio);
        configuracion.setOsmdroidTileCache(new File(directorio, "tiles"));
        configuracion.setUserAgentValue(BuildConfig.APPLICATION_ID);
    }
}
