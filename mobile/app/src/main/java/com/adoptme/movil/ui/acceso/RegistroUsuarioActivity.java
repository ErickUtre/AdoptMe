package com.adoptme.movil.ui.acceso;

import android.os.Bundle;

import androidx.activity.result.ActivityResultLauncher;
import androidx.appcompat.app.AppCompatActivity;

import com.adoptme.movil.databinding.ActivityRegistroUsuarioBinding;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.ubicacion.ContratoSeleccionUbicacion;

public final class RegistroUsuarioActivity extends AppCompatActivity {

    private RegistroUsuarioViewModel vistaModelo;

    private final ActivityResultLauncher<Ubicacion> seleccionUbicacion = registerForActivityResult(
            new ContratoSeleccionUbicacion(), ubicacion -> {
                if (ubicacion != null) {
                    vistaModelo.seleccionarUbicacion(ubicacion);
                }
            });

    @Override
    protected void onCreate(Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        ActivityRegistroUsuarioBinding vista = ActivityRegistroUsuarioBinding.inflate(getLayoutInflater());
        Pantallas.mostrarContenido(this, vista.getRoot());

        vistaModelo = Pantallas.vistaModelo(this, this, RegistroUsuarioViewModel.class);
        Pantallas.observarEstado(this, vistaModelo, vista.carga.getRoot(), this);
        vistaModelo.getUbicacion().observe(this, ubicacion -> vista.textoUbicacion.setText(Pantallas.describir(this, ubicacion)));
        vistaModelo.getRegistrado().observe(this, evento -> {
            if (evento.consumir() != null) {
                finish();
            }
        });

        vista.botonRegresar.setOnClickListener(boton -> finish());
        vista.botonUbicacion.setOnClickListener(boton -> seleccionUbicacion.launch(vistaModelo.getUbicacion().getValue()));
        vista.botonRegistrar.setOnClickListener(boton -> vistaModelo.registrar(
                Pantallas.texto(vista.campoNombre),
                Pantallas.texto(vista.campoCorreo),
                Pantallas.contrasena(vista.campoContrasena),
                Pantallas.contrasena(vista.campoConfirmacion),
                Pantallas.texto(vista.campoTelefono)));
    }
}
