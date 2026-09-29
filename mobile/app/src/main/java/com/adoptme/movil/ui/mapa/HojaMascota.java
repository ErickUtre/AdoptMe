package com.adoptme.movil.ui.mapa;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.os.BundleCompat;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.HojaMascotaBinding;
import com.adoptme.movil.dominio.AdopcionCercana;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.ui.adopciones.DetalleAdopcionFragment;
import com.adoptme.movil.ui.comun.Navegador;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

public final class HojaMascota extends BottomSheetDialogFragment {

    static final String ETIQUETA = "HojaMascota";
    private static final String ARGUMENTO_ADOPCION = "adopcion";

    static HojaMascota nueva(AdopcionCercana adopcion) {
        Bundle argumentos = new Bundle();
        argumentos.putParcelable(ARGUMENTO_ADOPCION, adopcion);
        HojaMascota hoja = new HojaMascota();
        hoja.setArguments(argumentos);
        return hoja;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        HojaMascotaBinding vista = HojaMascotaBinding.inflate(inflador, contenedor, false);
        AdopcionCercana adopcion = BundleCompat.getParcelable(requireArguments(), ARGUMENTO_ADOPCION, AdopcionCercana.class);
        if (adopcion == null) {
            dismiss();
            return vista.getRoot();
        }
        Mascota mascota = adopcion.getMascota();
        vista.textoNombre.setText(mascota.getNombre());
        vista.textoEdad.setText(getString(R.string.campo_valor, getString(R.string.edad), mascota.getEdad()));
        vista.textoSexo.setText(getString(R.string.campo_valor, getString(R.string.sexo), mascota.getSexo()));
        vista.textoEspecie.setText(getString(R.string.campo_valor, getString(R.string.especie), mascota.getEspecie()));
        vista.textoRaza.setText(getString(R.string.campo_valor, getString(R.string.raza), mascota.getRaza()));
        vista.botonVerDetalles.setOnClickListener(boton -> {
            ((Navegador) requireActivity()).mostrar(DetalleAdopcionFragment.nuevo(adopcion.getAdopcionId(), adopcion.getPublicadorId(), mascota), true);
            dismiss();
        });
        return vista.getRoot();
    }
}
