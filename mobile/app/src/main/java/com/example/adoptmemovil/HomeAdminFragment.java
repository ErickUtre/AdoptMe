package com.example.adoptmemovil;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

public class HomeAdminFragment extends Fragment {

    private LinearLayout btnReporteAdoptadas, btnReporteEnAdopcion;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_menuadmin, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        btnReporteAdoptadas = view.findViewById(R.id.btn_reporte_adoptadas);
        btnReporteEnAdopcion = view.findViewById(R.id.btn_reporte_en_adopcion);

        btnReporteAdoptadas.setOnClickListener(v -> {
            Bundle bundle = new Bundle();
            bundle.putBoolean("estado", true); // True = Adoptadas

            Fragment fragment = new ReporteFragment();
            fragment.setArguments(bundle);

            requireActivity().getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.fragment_container, fragment)
                    .addToBackStack(null)
                    .commit();
        });

        btnReporteEnAdopcion.setOnClickListener(v -> {
            Bundle bundle = new Bundle();
            bundle.putBoolean("estado", false); // False = Pendientes

            Fragment fragment = new ReporteFragment();
            fragment.setArguments(bundle);

            requireActivity().getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.fragment_container, fragment)
                    .addToBackStack(null)
                    .commit();
        });
    }
}
