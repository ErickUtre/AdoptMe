package com.example.adoptmemovil;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.FragmentActivity;

import com.example.adoptmemovil.modelo.Mascota;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

public class MascotaBottomSheet extends BottomSheetDialogFragment {

    private static final String ARG_MASCOTA = "mascota";

    public static MascotaBottomSheet newInstance(Mascota mascota) {
        MascotaBottomSheet fragment = new MascotaBottomSheet();
        Bundle args = new Bundle();
        args.putParcelable(ARG_MASCOTA, mascota);
        fragment.setArguments(args);
        return fragment;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.fragment_bottomsheet_mascota, container, false);

        Mascota mascota = getArguments() != null ? getArguments().getParcelable(ARG_MASCOTA) : null;

        TextView txtNombre = view.findViewById(R.id.txtNombre);
        TextView txtEdad = view.findViewById(R.id.txtEdad);
        TextView txtSexo = view.findViewById(R.id.txtSexo);
        TextView txtEspecie = view.findViewById(R.id.txtEspecie);
        TextView txtRaza = view.findViewById(R.id.txtRaza);
        Button btnVerDetalles = view.findViewById(R.id.btnVerDetalles);

        if (mascota != null) {
            txtNombre.setText(mascota.getNombre());
            txtEdad.setText("Edad: " + mascota.getEdad());
            txtSexo.setText("Edad: " + mascota.getSexo());
            txtEspecie.setText("Edad: " + mascota.getEspecie());
            txtRaza.setText("Raza: " + mascota.getRaza());
        }

        btnVerDetalles.setOnClickListener(v -> {
            ConsultarAdopcionFragment fragment = new ConsultarAdopcionFragment();
            Bundle bundle = new Bundle();
            bundle.putParcelable("mascota", mascota);
            bundle.putBoolean("desdeBottomSheet", true);
            fragment.setArguments(bundle);

            requireActivity().getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.fragment_container, fragment)
                    .addToBackStack(null)
                    .commit();

            dismiss();
        });

        return view;
    }
}
