package com.adoptme.movil.ui.comun;

import android.widget.ImageView;

import androidx.annotation.DrawableRes;

import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.bumptech.glide.Glide;
import com.bumptech.glide.load.model.GlideUrl;
import com.bumptech.glide.load.model.LazyHeaders;
import com.bumptech.glide.signature.ObjectKey;

import java.util.Collections;
import java.util.Map;

public final class RecursosRemotos {

    private final String urlBase;
    private final SesionUsuario sesion;

    public RecursosRemotos(String urlBase, SesionUsuario sesion) {
        this.urlBase = urlBase;
        this.sesion = sesion;
    }

    public String urlVideoMascota(int mascotaId) {
        return urlBase + "mascotas/" + mascotaId + "/video";
    }

    public Map<String, String> encabezadosAutenticacion() {
        String token = sesion.getToken();
        return token == null ? Collections.emptyMap() : Collections.singletonMap("Authorization", "Bearer " + token);
    }

    public void cargarFotoMascota(ImageView destino, int mascotaId, @DrawableRes int respaldo) {
        cargar(destino, urlBase + "mascotas/" + mascotaId + "/foto", respaldo, "mascota-" + mascotaId);
    }

    public void cargarFotoPerfil(ImageView destino, int version, @DrawableRes int respaldo) {
        cargar(destino, urlBase + "usuarios/yo/foto", respaldo, "perfil-" + sesion.getUsuarioId() + "-" + version);
    }

    private void cargar(ImageView destino, String url, @DrawableRes int respaldo, String firma) {
        LazyHeaders.Builder encabezados = new LazyHeaders.Builder();
        encabezadosAutenticacion().forEach(encabezados::addHeader);
        Glide.with(destino)
                .load(new GlideUrl(url, encabezados.build()))
                .signature(new ObjectKey(firma))
                .placeholder(respaldo)
                .error(respaldo)
                .centerCrop()
                .into(destino);
    }
}
