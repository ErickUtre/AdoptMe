package com.adoptme.movil.ui.adopciones;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.OptIn;
import androidx.fragment.app.Fragment;
import androidx.media3.common.MediaItem;
import androidx.media3.common.util.UnstableApi;
import androidx.media3.datasource.DefaultHttpDataSource;
import androidx.media3.exoplayer.ExoPlayer;
import androidx.media3.exoplayer.source.DefaultMediaSourceFactory;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.databinding.FragmentVideoBinding;
import com.adoptme.movil.ui.comun.RecursosRemotos;

public final class VideoMascotaFragment extends Fragment {

    private static final String ARGUMENTO_MASCOTA = "mascotaId";

    private FragmentVideoBinding vista;
    private ExoPlayer reproductor;

    static VideoMascotaFragment nuevo(int mascotaId) {
        Bundle argumentos = new Bundle();
        argumentos.putInt(ARGUMENTO_MASCOTA, mascotaId);
        VideoMascotaFragment fragmento = new VideoMascotaFragment();
        fragmento.setArguments(argumentos);
        return fragmento;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentVideoBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @OptIn(markerClass = UnstableApi.class)
    @Override
    public void onStart() {
        super.onStart();
        RecursosRemotos recursos = AdoptMeAplicacion.contenedor(requireContext()).getRecursosRemotos();
        DefaultHttpDataSource.Factory fuente = new DefaultHttpDataSource.Factory()
                .setDefaultRequestProperties(recursos.encabezadosAutenticacion());
        reproductor = new ExoPlayer.Builder(requireContext())
                .setMediaSourceFactory(new DefaultMediaSourceFactory(fuente))
                .build();
        reproductor.setMediaItem(MediaItem.fromUri(recursos.urlVideoMascota(requireArguments().getInt(ARGUMENTO_MASCOTA))));
        reproductor.prepare();
        reproductor.play();
        vista.reproductor.setPlayer(reproductor);
    }

    @Override
    public void onStop() {
        vista.reproductor.setPlayer(null);
        reproductor.release();
        reproductor = null;
        super.onStop();
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }
}
