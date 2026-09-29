package com.adoptme.movil.ui.adopciones;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.DiffUtil;
import androidx.recyclerview.widget.ListAdapter;
import androidx.recyclerview.widget.RecyclerView;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.ElementoAdopcionBinding;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.comun.RecursosRemotos;

import java.util.Objects;
import java.util.function.Consumer;

final class AdaptadorAdopciones extends ListAdapter<Adopcion, AdaptadorAdopciones.Tarjeta> {

    private static final DiffUtil.ItemCallback<Adopcion> COMPARADOR = new DiffUtil.ItemCallback<>() {
        @Override
        public boolean areItemsTheSame(@NonNull Adopcion anterior, @NonNull Adopcion nueva) {
            return anterior.getAdopcionId() == nueva.getAdopcionId();
        }

        @Override
        public boolean areContentsTheSame(@NonNull Adopcion anterior, @NonNull Adopcion nueva) {
            return anterior.estaAdoptada() == nueva.estaAdoptada() && Objects.equals(anterior.getMascota(), nueva.getMascota());
        }
    };

    private final RecursosRemotos recursos;
    private final Consumer<Adopcion> alVerSolicitudes;
    private final Consumer<Adopcion> alVerDetalles;
    private final Consumer<Adopcion> alEliminar;

    AdaptadorAdopciones(RecursosRemotos recursos, Consumer<Adopcion> alVerSolicitudes, Consumer<Adopcion> alVerDetalles, Consumer<Adopcion> alEliminar) {
        super(COMPARADOR);
        this.recursos = recursos;
        this.alVerSolicitudes = alVerSolicitudes;
        this.alVerDetalles = alVerDetalles;
        this.alEliminar = alEliminar;
    }

    @NonNull
    @Override
    public Tarjeta onCreateViewHolder(@NonNull ViewGroup padre, int tipo) {
        return new Tarjeta(ElementoAdopcionBinding.inflate(LayoutInflater.from(padre.getContext()), padre, false));
    }

    @Override
    public void onBindViewHolder(@NonNull Tarjeta tarjeta, int posicion) {
        tarjeta.mostrar(getItem(posicion));
    }

    final class Tarjeta extends RecyclerView.ViewHolder {

        private final ElementoAdopcionBinding vista;

        Tarjeta(ElementoAdopcionBinding vista) {
            super(vista.getRoot());
            this.vista = vista;
        }

        void mostrar(Adopcion adopcion) {
            Context contexto = vista.getRoot().getContext();
            Mascota mascota = adopcion.getMascota();
            vista.textoNombre.setText(mascota == null ? Pantallas.valor(null) : mascota.getNombre());
            vista.textoResumen.setText(mascota == null ? Pantallas.valor(null) : contexto.getString(R.string.resumen_mascota,
                    Pantallas.valor(mascota.getEspecie()), Pantallas.valor(mascota.getRaza()), Pantallas.valor(mascota.getEdad())));
            vista.textoEstado.setText(adopcion.estaAdoptada() ? R.string.estado_adoptado : R.string.estado_disponible);
            vista.textoEstado.setTextColor(ContextCompat.getColor(contexto, adopcion.estaAdoptada() ? R.color.adoptado : R.color.disponible));
            recursos.cargarFotoMascota(vista.imagenMascota, adopcion.getMascotaId(), R.drawable.defaultpet);
            vista.botonSolicitudes.setEnabled(!adopcion.estaAdoptada());
            vista.botonSolicitudes.setOnClickListener(boton -> alVerSolicitudes.accept(adopcion));
            vista.botonDetalles.setOnClickListener(boton -> alVerDetalles.accept(adopcion));
            vista.botonEliminar.setOnClickListener(boton -> alEliminar.accept(adopcion));
        }
    }
}
