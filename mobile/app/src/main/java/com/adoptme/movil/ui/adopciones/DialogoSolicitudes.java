package com.adoptme.movil.ui.adopciones;

import android.app.Dialog;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.DialogFragment;
import androidx.recyclerview.widget.DiffUtil;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.ListAdapter;
import androidx.recyclerview.widget.RecyclerView;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.DialogoSolicitudesBinding;
import com.adoptme.movil.databinding.ElementoSolicitudBinding;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.ui.comun.Pantallas;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

import java.util.function.Consumer;

public final class DialogoSolicitudes extends DialogFragment {

    static final String ETIQUETA = "DialogoSolicitudes";
    static final String RESULTADO = "solicitudesActualizadas";
    private static final String ARGUMENTO_ADOPCION = "adopcionId";

    static DialogoSolicitudes nuevo(int adopcionId) {
        Bundle argumentos = new Bundle();
        argumentos.putInt(ARGUMENTO_ADOPCION, adopcionId);
        DialogoSolicitudes dialogo = new DialogoSolicitudes();
        dialogo.setArguments(argumentos);
        return dialogo;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(@Nullable Bundle estadoGuardado) {
        DialogoSolicitudesBinding vista = DialogoSolicitudesBinding.inflate(requireActivity().getLayoutInflater());
        SolicitudesViewModel vistaModelo = Pantallas.vistaModelo(this, requireContext(), SolicitudesViewModel.class);
        AdaptadorSolicitudes adaptador = new AdaptadorSolicitudes(
                solicitud -> confirmarAceptacion(vistaModelo, solicitud),
                vistaModelo::rechazar);
        vista.listaSolicitudes.setLayoutManager(new LinearLayoutManager(requireContext()));
        vista.listaSolicitudes.setAdapter(adaptador);

        Pantallas.observarEstado(this, vistaModelo, vista.progreso, requireContext());
        vistaModelo.getPendientes().observe(this, solicitudes -> {
            adaptador.submitList(solicitudes);
            vista.textoVacio.setVisibility(solicitudes.isEmpty() ? View.VISIBLE : View.GONE);
        });
        vistaModelo.getAdopcionAceptada().observe(this, evento -> {
            if (evento.consumir() != null) {
                getParentFragmentManager().setFragmentResult(RESULTADO, Bundle.EMPTY);
                dismiss();
            }
        });
        if (estadoGuardado == null) {
            vistaModelo.cargar(requireArguments().getInt(ARGUMENTO_ADOPCION));
        }
        return new MaterialAlertDialogBuilder(requireContext())
                .setTitle(R.string.solicitudes_pendientes)
                .setView(vista.getRoot())
                .setNegativeButton(R.string.regresar, null)
                .create();
    }

    private void confirmarAceptacion(SolicitudesViewModel vistaModelo, Solicitud solicitud) {
        new MaterialAlertDialogBuilder(requireContext())
                .setTitle(R.string.confirmacion)
                .setMessage(getString(R.string.confirmar_aceptar, solicitud.getNombreAdoptante()))
                .setPositiveButton(R.string.aceptar, (dialogo, boton) -> vistaModelo.aceptar(solicitud))
                .setNegativeButton(R.string.cancelar, null)
                .show();
    }

    private static final class AdaptadorSolicitudes extends ListAdapter<Solicitud, AdaptadorSolicitudes.Fila> {

        private static final DiffUtil.ItemCallback<Solicitud> COMPARADOR = new DiffUtil.ItemCallback<>() {
            @Override
            public boolean areItemsTheSame(@NonNull Solicitud anterior, @NonNull Solicitud nueva) {
                return anterior.getSolicitudId() == nueva.getSolicitudId();
            }

            @Override
            public boolean areContentsTheSame(@NonNull Solicitud anterior, @NonNull Solicitud nueva) {
                return anterior.getNombreAdoptante().equals(nueva.getNombreAdoptante());
            }
        };

        private final Consumer<Solicitud> alAceptar;
        private final Consumer<Solicitud> alRechazar;

        AdaptadorSolicitudes(Consumer<Solicitud> alAceptar, Consumer<Solicitud> alRechazar) {
            super(COMPARADOR);
            this.alAceptar = alAceptar;
            this.alRechazar = alRechazar;
        }

        @NonNull
        @Override
        public Fila onCreateViewHolder(@NonNull ViewGroup padre, int tipo) {
            return new Fila(ElementoSolicitudBinding.inflate(LayoutInflater.from(padre.getContext()), padre, false));
        }

        @Override
        public void onBindViewHolder(@NonNull Fila fila, int posicion) {
            Solicitud solicitud = getItem(posicion);
            fila.vista.textoAdoptante.setText(solicitud.getNombreAdoptante());
            fila.vista.botonAceptar.setOnClickListener(boton -> alAceptar.accept(solicitud));
            fila.vista.botonRechazar.setOnClickListener(boton -> alRechazar.accept(solicitud));
        }

        static final class Fila extends RecyclerView.ViewHolder {
            private final ElementoSolicitudBinding vista;

            Fila(ElementoSolicitudBinding vista) {
                super(vista.getRoot());
                this.vista = vista;
            }
        }
    }
}
