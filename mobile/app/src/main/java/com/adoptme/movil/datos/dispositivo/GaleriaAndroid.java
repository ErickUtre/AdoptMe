package com.adoptme.movil.datos.dispositivo;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Context;
import android.graphics.Bitmap;
import android.media.MediaScannerConnection;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;

import androidx.annotation.RequiresApi;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.Galeria;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.concurrent.Executor;

public final class GaleriaAndroid implements Galeria {

    private static final String TIPO_PNG = "image/png";
    private static final String EXTENSION_PNG = ".png";
    private static final String CARPETA = "AdoptMe";
    private static final int CALIDAD = 100;

    private final Context contexto;
    private final Executor ejecutor;

    public GaleriaAndroid(Context contexto, Executor ejecutor) {
        this.contexto = contexto.getApplicationContext();
        this.ejecutor = ejecutor;
    }

    @Override
    public void guardarPng(Bitmap imagen, String nombre, AlTerminar<Void> alTerminar) {
        ejecutor.execute(() -> {
            try {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
                    guardarEnMediaStore(imagen, nombre + EXTENSION_PNG);
                } else {
                    guardarEnDirectorioPropio(imagen, nombre + EXTENSION_PNG);
                }
                alTerminar.con(Resultado.exito(null));
            } catch (IOException excepcion) {
                alTerminar.con(Resultado.fallo(TipoError.DESCONOCIDO, excepcion.getMessage()));
            }
        });
    }

    @RequiresApi(Build.VERSION_CODES.Q)
    private void guardarEnMediaStore(Bitmap imagen, String archivo) throws IOException {
        ContentResolver resolvedor = contexto.getContentResolver();
        ContentValues valores = new ContentValues();
        valores.put(MediaStore.Images.Media.DISPLAY_NAME, archivo);
        valores.put(MediaStore.Images.Media.MIME_TYPE, TIPO_PNG);
        valores.put(MediaStore.Images.Media.RELATIVE_PATH, Environment.DIRECTORY_PICTURES + File.separator + CARPETA);
        valores.put(MediaStore.Images.Media.IS_PENDING, 1);
        Uri destino = resolvedor.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, valores);
        if (destino == null) {
            throw new IOException(archivo);
        }
        try (OutputStream salida = resolvedor.openOutputStream(destino)) {
            escribir(imagen, salida);
        } catch (IOException excepcion) {
            resolvedor.delete(destino, null, null);
            throw excepcion;
        }
        valores.clear();
        valores.put(MediaStore.Images.Media.IS_PENDING, 0);
        resolvedor.update(destino, valores, null, null);
    }

    private void guardarEnDirectorioPropio(Bitmap imagen, String archivo) throws IOException {
        File carpeta = contexto.getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        if (carpeta == null || (!carpeta.exists() && !carpeta.mkdirs())) {
            throw new IOException(archivo);
        }
        File destino = new File(carpeta, archivo);
        try (OutputStream salida = new FileOutputStream(destino)) {
            escribir(imagen, salida);
        }
        MediaScannerConnection.scanFile(contexto, new String[]{destino.getAbsolutePath()}, new String[]{TIPO_PNG}, null);
    }

    private static void escribir(Bitmap imagen, OutputStream salida) throws IOException {
        if (salida == null || !imagen.compress(Bitmap.CompressFormat.PNG, CALIDAD, salida)) {
            throw new IOException(TIPO_PNG);
        }
    }
}
