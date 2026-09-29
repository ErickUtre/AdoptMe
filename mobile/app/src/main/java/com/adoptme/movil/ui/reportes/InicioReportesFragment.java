package com.adoptme.movil.ui.reportes;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.databinding.FragmentInicioReportesBinding;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.ui.comun.Navegador;

public final class InicioReportesFragment extends Fragment {

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        FragmentInicioReportesBinding vista = FragmentInicioReportesBinding.inflate(inflador, contenedor, false);
        vista.tarjetaAdoptadas.setOnClickListener(tarjeta -> abrir(EstadoAdopcion.ADOPTADA));
        vista.tarjetaEnAdopcion.setOnClickListener(tarjeta -> abrir(EstadoAdopcion.DISPONIBLE));
        return vista.getRoot();
    }

    private void abrir(EstadoAdopcion estado) {
        ((Navegador) requireActivity()).mostrar(ReporteFragment.nuevo(estado), true);
    }
}
