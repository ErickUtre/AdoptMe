package com.adoptme.movil.ui.adopciones;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Spinner;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.PickVisualMediaRequest;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.ArrayRes;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.DialogoEdadBinding;
import com.adoptme.movil.databinding.FragmentRegistroAdopcionBinding;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.adopciones.RegistroAdopcionViewModel.FormularioMascota;
import com.adoptme.movil.ui.comun.Navegador;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.ubicacion.ContratoSeleccionUbicacion;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

public final class RegistroAdopcionFragment extends Fragment {

    private static final int ANIOS_MAXIMOS = 30;
    private static final int MESES_MAXIMOS = 11;

    private FragmentRegistroAdopcionBinding vista;
    private RegistroAdopcionViewModel vistaModelo;

    private final ActivityResultLauncher<PickVisualMediaRequest> seleccionFoto = registerForActivityResult(
            new ActivityResultContracts.PickVisualMedia(), archivo -> {
                if (archivo != null) {
                    vistaModelo.seleccionarFoto(archivo);
                }
            });
    private final ActivityResultLauncher<PickVisualMediaRequest> seleccionVideo = registerForActivityResult(
            new ActivityResultContracts.PickVisualMedia(), archivo -> {
                if (archivo != null) {
                    vistaModelo.seleccionarVideo(archivo);
                }
            });
    private final ActivityResultLauncher<Ubicacion> seleccionUbicacion = registerForActivityResult(
            new ContratoSeleccionUbicacion(), ubicacion -> {
                if (ubicacion != null) {
                    vistaModelo.seleccionarUbicacion(ubicacion);
                }
            });
    private int aniosSeleccionados;
    private int mesesSeleccionados;

    @Override
    public void onCreate(@Nullable Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        vistaModelo = Pantallas.vistaModelo(this, requireContext(), RegistroAdopcionViewModel.class);
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentRegistroAdopcionBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        Pantallas.observarEstado(getViewLifecycleOwner(), vistaModelo, vista.carga.getRoot(), requireContext());
        configurarSelector(vista.selectorSexo, R.array.sexos);
        configurarSelector(vista.selectorTamano, R.array.tamanos);

        vista.campoEdad.setOnClickListener(campo -> mostrarSelectorEdad());
        vista.botonFoto.setOnClickListener(boton -> seleccionFoto.launch(solicitud(ActivityResultContracts.PickVisualMedia.ImageOnly.INSTANCE)));
        vista.botonVideo.setOnClickListener(boton -> seleccionVideo.launch(solicitud(ActivityResultContracts.PickVisualMedia.VideoOnly.INSTANCE)));
        vista.botonUbicacion.setOnClickListener(boton -> seleccionUbicacion.launch(vistaModelo.getUbicacion().getValue()));
        vista.botonRegistrar.setOnClickListener(boton -> vistaModelo.registrar(leerFormulario()));

        vistaModelo.getFoto().observe(getViewLifecycleOwner(), foto -> {
            vista.vistaPreviaFoto.setImageURI(foto);
            vista.vistaPreviaFoto.setVisibility(foto == null ? View.GONE : View.VISIBLE);
        });
        vistaModelo.getVideo().observe(getViewLifecycleOwner(), video ->
                vista.textoVideo.setVisibility(video == null ? View.GONE : View.VISIBLE));
        vistaModelo.getUbicacion().observe(getViewLifecycleOwner(), ubicacion -> {
            vista.textoUbicacion.setText(ubicacion == null ? "" : Pantallas.describir(requireContext(), ubicacion));
            vista.textoUbicacion.setVisibility(ubicacion == null ? View.GONE : View.VISIBLE);
        });
        vistaModelo.getRegistrada().observe(getViewLifecycleOwner(), evento -> {
            if (evento.consumir() != null) {
                ((Navegador) requireActivity()).mostrar(new AdopcionesPropiasFragment(), false);
            }
        });
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }

    private void configurarSelector(Spinner selector, @ArrayRes int opciones) {
        ArrayAdapter<CharSequence> adaptador = ArrayAdapter.createFromResource(requireContext(), opciones, android.R.layout.simple_spinner_item);
        adaptador.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        selector.setAdapter(adaptador);
    }

    private static PickVisualMediaRequest solicitud(ActivityResultContracts.PickVisualMedia.VisualMediaType tipo) {
        return new PickVisualMediaRequest.Builder().setMediaType(tipo).build();
    }

    private void mostrarSelectorEdad() {
        DialogoEdadBinding dialogo = DialogoEdadBinding.inflate(getLayoutInflater());
        dialogo.selectorAnios.setMaxValue(ANIOS_MAXIMOS);
        dialogo.selectorMeses.setMaxValue(MESES_MAXIMOS);
        dialogo.selectorAnios.setValue(aniosSeleccionados);
        dialogo.selectorMeses.setValue(mesesSeleccionados);
        new MaterialAlertDialogBuilder(requireContext())
                .setTitle(R.string.selecciona_edad)
                .setView(dialogo.getRoot())
                .setPositiveButton(R.string.aceptar, (ventana, boton) -> {
                    aniosSeleccionados = dialogo.selectorAnios.getValue();
                    mesesSeleccionados = dialogo.selectorMeses.getValue();
                    vista.campoEdad.setText(getString(R.string.formato_edad,
                            getResources().getQuantityString(R.plurals.anios, aniosSeleccionados, aniosSeleccionados),
                            getResources().getQuantityString(R.plurals.meses, mesesSeleccionados, mesesSeleccionados)));
                })
                .setNegativeButton(R.string.cancelar, null)
                .show();
    }

    private FormularioMascota leerFormulario() {
        return new FormularioMascota(
                Pantallas.texto(vista.campoNombre),
                Pantallas.texto(vista.campoEspecie),
                Pantallas.texto(vista.campoRaza),
                Pantallas.texto(vista.campoEdad),
                seleccion(vista.selectorSexo),
                seleccion(vista.selectorTamano),
                Pantallas.texto(vista.campoDescripcion));
    }

    private static String seleccion(Spinner selector) {
        Object elegido = selector.getSelectedItem();
        return elegido == null ? null : elegido.toString();
    }
}
