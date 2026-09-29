package com.adoptme.movil.ui.perfil;

import android.os.Bundle;
import android.text.InputType;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.PickVisualMediaRequest;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.StringRes;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.R;
import com.adoptme.movil.databinding.DialogoEditarCampoBinding;
import com.adoptme.movil.databinding.FragmentPerfilBinding;
import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.ubicacion.ContratoSeleccionUbicacion;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;

import java.util.function.Consumer;
import java.util.function.Function;

public final class PerfilFragment extends Fragment {

    private FragmentPerfilBinding vista;
    private PerfilViewModel vistaModelo;

    private final ActivityResultLauncher<PickVisualMediaRequest> seleccionFoto = registerForActivityResult(
            new ActivityResultContracts.PickVisualMedia(), archivo -> {
                if (archivo != null) {
                    vistaModelo.cambiarFoto(archivo);
                }
            });
    private final ActivityResultLauncher<Ubicacion> seleccionUbicacion = registerForActivityResult(
            new ContratoSeleccionUbicacion(), ubicacion -> {
                if (ubicacion != null) {
                    vistaModelo.actualizarUbicacion(ubicacion);
                }
            });


    @Override
    public void onCreate(@Nullable Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        vistaModelo = Pantallas.vistaModelo(this, requireContext(), PerfilViewModel.class);
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentPerfilBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        Pantallas.observarEstado(getViewLifecycleOwner(), vistaModelo, vista.carga.getRoot(), requireContext());
        vistaModelo.getPerfil().observe(getViewLifecycleOwner(), this::mostrar);
        vistaModelo.getVersionFoto().observe(getViewLifecycleOwner(), version ->
                AdoptMeAplicacion.contenedor(requireContext()).getRecursosRemotos()
                        .cargarFotoPerfil(vista.imagenPerfil, version, R.drawable.ic_perfil_defecto));

        vista.botonEditarFoto.setOnClickListener(boton -> seleccionFoto.launch(new PickVisualMediaRequest.Builder()
                .setMediaType(ActivityResultContracts.PickVisualMedia.ImageOnly.INSTANCE)
                .build()));
        vista.botonEditarNombre.setOnClickListener(boton -> editar(R.string.nombre, InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_FLAG_CAP_WORDS,
                vistaModelo::validarNombre, vistaModelo::actualizarNombre));
        vista.botonEditarCorreo.setOnClickListener(boton -> editar(R.string.correo, InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS,
                vistaModelo::validarCorreo, vistaModelo::actualizarCorreo));
        vista.botonEditarTelefono.setOnClickListener(boton -> editar(R.string.telefono, InputType.TYPE_CLASS_PHONE,
                vistaModelo::validarTelefono, vistaModelo::actualizarTelefono));
        vista.botonUbicacion.setOnClickListener(boton -> {
            Perfil perfil = vistaModelo.getPerfil().getValue();
            seleccionUbicacion.launch(perfil == null ? null : perfil.getUbicacion());
        });
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }

    private void mostrar(Perfil perfil) {
        if (perfil == null) {
            return;
        }
        vista.textoNombre.setText(perfil.getNombre());
        vista.textoCorreo.setText(perfil.getAcceso() == null ? Pantallas.valor(null) : perfil.getAcceso().getCorreo());
        vista.textoTelefono.setText(perfil.getTelefono());
        Ubicacion ubicacion = perfil.getUbicacion();
        vista.textoUbicacion.setText(ubicacion == null ? Pantallas.valor(null) : Pantallas.describir(requireContext(), ubicacion));
        vista.botonUbicacion.setText(ubicacion == null ? R.string.registrar_ubicacion : R.string.actualizar_ubicacion);
    }

    private void editar(@StringRes int campo, int tipoEntrada, Function<String, Integer> validar, Consumer<String> guardar) {
        DialogoEditarCampoBinding dialogo = DialogoEditarCampoBinding.inflate(getLayoutInflater());
        dialogo.contenedorCampo.setHint(campo);
        dialogo.campoValor.setInputType(tipoEntrada);
        AlertDialog ventana = new MaterialAlertDialogBuilder(requireContext())
                .setTitle(getString(R.string.editar_campo, getString(campo)))
                .setView(dialogo.getRoot())
                .setPositiveButton(R.string.guardar, null)
                .setNegativeButton(R.string.cancelar, null)
                .create();
        ventana.setOnShowListener(mostrada -> ventana.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(boton -> {
            String valor = Pantallas.texto(dialogo.campoValor);
            Integer error = validar.apply(valor);
            if (error != null) {
                dialogo.contenedorCampo.setError(getString(error));
                return;
            }
            guardar.accept(valor);
            ventana.dismiss();
        }));
        ventana.show();
    }
}
