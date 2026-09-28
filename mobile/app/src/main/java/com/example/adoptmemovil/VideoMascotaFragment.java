package com.example.adoptmemovil;

import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.media3.ui.PlayerView;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.example.adoptmemovil.utilidades.InterfazUsuarioUtils;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

public class VideoMascotaFragment extends Fragment {

    private PlayerView playerView;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_video_mascota, container, false);
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        playerView = view.findViewById(R.id.videoView);

        int mascotaId = getArguments().getInt("mascotaId");
        String token = UsuarioSingleton.getInstancia().getToken();

        InterfazUsuarioUtils.reproducirVideoMascota(requireContext(), token, mascotaId, playerView);
    }

    @Override
    public void onDestroyView() {
        if (playerView != null && playerView.getPlayer() != null) {
            playerView.getPlayer().release();
            playerView.setPlayer(null);
        }
        super.onDestroyView();
    }
}
