package com.adoptme.movil.ui.adopciones;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.R;
import com.adoptme.movil.databinding.FragmentAdopcionesPropiasBinding;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.ui.comun.Navegador;
import com.adoptme.movil.ui.comun.Pantallas;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

public final class AdopcionesPropiasFragment extends Fragment {

    private FragmentAdopcionesPropiasBinding vista;
    private AdopcionesPropiasViewModel vistaModelo;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentAdopcionesPropiasBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        vistaModelo = Pantallas.vistaModelo(this, requireContext(), AdopcionesPropiasViewModel.class);
        Pantallas.observarEstado(getViewLifecycleOwner(), vistaModelo, vista.carga.getRoot(), requireContext());

        AdaptadorAdopciones adaptador = new AdaptadorAdopciones(
                AdoptMeAplicacion.contenedor(requireContext()).getRecursosRemotos(),
                this::verSolicitudes,
                this::verDetalles,
                this::confirmarEliminacion);
        vista.listaAdopciones.setLayoutManager(new LinearLayoutManager(requireContext()));
        vista.listaAdopciones.setAdapter(adaptador);

        vistaModelo.getLista().observe(getViewLifecycleOwner(), adopciones -> {
            adaptador.submitList(adopciones);
            vista.textoVacio.setVisibility(adopciones.isEmpty() ? View.VISIBLE : View.GONE);
        });
        getChildFragmentManager().setFragmentResultListener(DialogoSolicitudes.RESULTADO, getViewLifecycleOwner(),
                (clave, resultado) -> vistaModelo.cargar());
        vistaModelo.cargar();
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }

    private void verSolicitudes(Adopcion adopcion) {
        DialogoSolicitudes.nuevo(adopcion.getAdopcionId()).show(getChildFragmentManager(), DialogoSolicitudes.ETIQUETA);
    }

    private void verDetalles(Adopcion adopcion) {
        ((Navegador) requireActivity()).mostrar(
                DetalleAdopcionFragment.nuevo(adopcion.getAdopcionId(), adopcion.getPublicadorId(), adopcion.getMascota()), true);
    }

    private void confirmarEliminacion(Adopcion adopcion) {
        new MaterialAlertDialogBuilder(requireContext())
                .setTitle(R.string.confirmacion)
                .setMessage(getString(R.string.confirmar_eliminar, adopcion.getMascota() == null ? "" : adopcion.getMascota().getNombre()))
                .setPositiveButton(R.string.eliminar, (dialogo, boton) -> vistaModelo.eliminar(adopcion))
                .setNegativeButton(R.string.cancelar, null)
                .show();
    }
}
