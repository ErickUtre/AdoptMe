package com.adoptme.movil.ui.mapa;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;

import androidx.annotation.DrawableRes;
import androidx.core.content.ContextCompat;

import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.MapView;
import org.osmdroid.views.overlay.Marker;

public final class Marcadores {

    private static final int TAMANO_ICONO_DP = 40;

    private Marcadores() {
    }

    public static Marker crear(MapView mapa, GeoPoint punto, @DrawableRes int icono, String titulo) {
        Marker marcador = new Marker(mapa);
        marcador.setPosition(punto);
        marcador.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);
        marcador.setIcon(escalar(mapa.getContext(), icono));
        marcador.setTitle(titulo);
        return marcador;
    }

    private static Drawable escalar(Context contexto, @DrawableRes int icono) {
        Drawable original = ContextCompat.getDrawable(contexto, icono);
        if (original == null) {
            return null;
        }
        int tamano = Math.round(TAMANO_ICONO_DP * contexto.getResources().getDisplayMetrics().density);
        Bitmap mapaBits = Bitmap.createBitmap(tamano, tamano, Bitmap.Config.ARGB_8888);
        Canvas lienzo = new Canvas(mapaBits);
        original.setBounds(0, 0, tamano, tamano);
        original.draw(lienzo);
        return new BitmapDrawable(contexto.getResources(), mapaBits);
    }
}
