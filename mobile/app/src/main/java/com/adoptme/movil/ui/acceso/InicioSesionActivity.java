package com.adoptme.movil.ui.acceso;

import android.content.Intent;
import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;

import com.adoptme.movil.databinding.ActivityInicioSesionBinding;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.principal.PrincipalActivity;

public final class InicioSesionActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        ActivityInicioSesionBinding vista = ActivityInicioSesionBinding.inflate(getLayoutInflater());
        Pantallas.mostrarContenido(this, vista.getRoot());

        InicioSesionViewModel vistaModelo = Pantallas.vistaModelo(this, this, InicioSesionViewModel.class);
        Pantallas.observarEstado(this, vistaModelo, vista.carga.getRoot(), this);
        vistaModelo.getSesionIniciada().observe(this, evento -> {
            if (evento.consumir() != null) {
                startActivity(new Intent(this, PrincipalActivity.class));
                finish();
            }
        });

        vista.botonIniciarSesion.setOnClickListener(boton ->
                vistaModelo.iniciarSesion(Pantallas.texto(vista.campoCorreo), Pantallas.contrasena(vista.campoContrasena)));
        vista.enlaceRegistro.setOnClickListener(enlace -> startActivity(new Intent(this, RegistroUsuarioActivity.class)));
    }
}
