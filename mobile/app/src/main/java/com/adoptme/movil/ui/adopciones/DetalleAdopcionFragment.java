package com.adoptme.movil.ui.adopciones;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.os.BundleCompat;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.R;
import com.adoptme.movil.databinding.FragmentDetalleAdopcionBinding;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.ui.comun.Navegador;
import com.adoptme.movil.ui.comun.Pantallas;

public final class DetalleAdopcionFragment extends Fragment {

    private static final String ARGUMENTO_ADOPCION = "adopcionId";
    private static final String ARGUMENTO_PUBLICADOR = "publicadorId";
    private static final String ARGUMENTO_MASCOTA = "mascota";

    private FragmentDetalleAdopcionBinding vista;

    public static DetalleAdopcionFragment nuevo(int adopcionId, int publicadorId, Mascota mascota) {
        Bundle argumentos = new Bundle();
        argumentos.putInt(ARGUMENTO_ADOPCION, adopcionId);
        argumentos.putInt(ARGUMENTO_PUBLICADOR, publicadorId);
        argumentos.putParcelable(ARGUMENTO_MASCOTA, mascota);
        DetalleAdopcionFragment fragmento = new DetalleAdopcionFragment();
        fragmento.setArguments(argumentos);
        return fragmento;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentDetalleAdopcionBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        Bundle argumentos = requireArguments();
        int adopcionId = argumentos.getInt(ARGUMENTO_ADOPCION);
        int publicadorId = argumentos.getInt(ARGUMENTO_PUBLICADOR);
        Mascota mascota = BundleCompat.getParcelable(argumentos, ARGUMENTO_MASCOTA, Mascota.class);

        DetalleAdopcionViewModel vistaModelo = Pantallas.vistaModelo(this, requireContext(), DetalleAdopcionViewModel.class);
        Pantallas.observarEstado(getViewLifecycleOwner(), vistaModelo, vista.carga.getRoot(), requireContext());

        if (mascota != null) {
            mostrar(mascota);
            vista.botonVerVideo.setOnClickListener(boton ->
                    ((Navegador) requireActivity()).mostrar(VideoMascotaFragment.nuevo(mascota.getMascotaId()), true));
        }
        vista.botonSolicitar.setVisibility(vistaModelo.puedeSolicitar(publicadorId) ? View.VISIBLE : View.GONE);
        vista.botonSolicitar.setOnClickListener(boton -> vistaModelo.solicitar(adopcionId));
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }

    private void mostrar(Mascota mascota) {
        vista.textoNombre.setText(getString(R.string.campo_valor, getString(R.string.nombre), mascota.getNombre()));
        vista.textoEspecie.setText(getString(R.string.campo_valor, getString(R.string.especie), mascota.getEspecie()));
        vista.textoRaza.setText(getString(R.string.campo_valor, getString(R.string.raza), mascota.getRaza()));
        vista.textoEdad.setText(getString(R.string.campo_valor, getString(R.string.edad), mascota.getEdad()));
        vista.textoSexo.setText(getString(R.string.campo_valor, getString(R.string.sexo), mascota.getSexo()));
        vista.textoTamano.setText(getString(R.string.campo_valor, getString(R.string.tamano), mascota.getTamano()));
        vista.textoDescripcion.setText(Pantallas.valor(mascota.getDescripcion()));
        AdoptMeAplicacion.contenedor(requireContext()).getRecursosRemotos()
                .cargarFotoMascota(vista.imagenMascota, mascota.getMascotaId(), R.drawable.defaultpet);
    }
}
