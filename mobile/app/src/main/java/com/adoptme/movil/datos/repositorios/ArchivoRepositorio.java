package com.adoptme.movil.datos.repositorios;

import android.net.Uri;

import com.adoptme.movil.comun.AlTerminar;

public interface ArchivoRepositorio {

    void subirFotoPerfil(Uri archivo, AlTerminar<Void> alTerminar);

    void subirFotoMascota(int mascotaId, Uri archivo, AlTerminar<Void> alTerminar);

    void subirVideoMascota(int mascotaId, Uri archivo, AlTerminar<Void> alTerminar);
}
