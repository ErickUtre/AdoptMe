package com.example.adoptmemovil.utilidades;

import android.graphics.Bitmap;

import java.util.HashMap;
import java.util.Map;

public class FotoMascotaCache {
    private static final Map<Integer, Bitmap> cache = new HashMap<>();

    public static void guardarFoto(int mascotaId, Bitmap bitmap) {
        cache.put(mascotaId, bitmap);
    }

    public static Bitmap obtenerFoto(int mascotaId) {
        return cache.get(mascotaId);
    }

    public static boolean contieneFoto(int mascotaId) {
        return cache.containsKey(mascotaId);
    }

    public static void limpiarCache() {
        cache.clear();
    }
}
