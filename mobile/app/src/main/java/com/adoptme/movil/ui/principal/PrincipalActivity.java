package com.adoptme.movil.ui.principal;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.R;
import com.adoptme.movil.databinding.ActivityPrincipalBinding;
import com.adoptme.movil.ui.acceso.InicioSesionActivity;
import com.adoptme.movil.ui.adopciones.AdopcionesPropiasFragment;
import com.adoptme.movil.ui.adopciones.RegistroAdopcionFragment;
import com.adoptme.movil.ui.comun.Navegador;
import com.adoptme.movil.ui.comun.Pantallas;
import com.adoptme.movil.ui.mapa.MapaFragment;
import com.adoptme.movil.ui.perfil.PerfilFragment;
import com.adoptme.movil.ui.reportes.InicioReportesFragment;

import java.util.function.Supplier;

public final class PrincipalActivity extends AppCompatActivity implements Navegador {

    private PrincipalViewModel vistaModelo;

    @Override
    protected void onCreate(Bundle estadoGuardado) {
        super.onCreate(estadoGuardado);
        vistaModelo = Pantallas.vistaModelo(this, this, PrincipalViewModel.class);
        if (!vistaModelo.haySesion()) {
            volverAInicioSesion();
            return;
        }

        ActivityPrincipalBinding vista = ActivityPrincipalBinding.inflate(getLayoutInflater());
        Pantallas.mostrarContenido(this, vista.getRoot());

        boolean administrador = vistaModelo.esAdministrador();
        Supplier<Fragment> inicio = administrador ? InicioReportesFragment::new : MapaFragment::new;
        int visibilidadUsuario = administrador ? View.GONE : View.VISIBLE;
        vista.botonRegistrarAdopcion.setVisibility(visibilidadUsuario);
        vista.botonMisAdopciones.setVisibility(visibilidadUsuario);

        vista.botonInicio.setOnClickListener(boton -> mostrar(inicio.get(), false));
        vista.botonRegistrarAdopcion.setOnClickListener(boton -> mostrar(new RegistroAdopcionFragment(), false));
        vista.botonMisAdopciones.setOnClickListener(boton -> mostrar(new AdopcionesPropiasFragment(), false));
        vista.botonPerfil.setOnClickListener(boton -> mostrar(new PerfilFragment(), false));
        vista.botonCerrarSesion.setOnClickListener(boton -> {
            vistaModelo.cerrarSesion();
            volverAInicioSesion();
        });

        vistaModelo.getVersionFoto().observe(this, version -> AdoptMeAplicacion.contenedor(this).getRecursosRemotos()
                .cargarFotoPerfil(vista.botonPerfil, version, R.drawable.ic_perfil_defecto));

        if (estadoGuardado == null) {
            mostrar(inicio.get(), false);
        }
    }

    @Override
    public void mostrar(Fragment destino, boolean conservarAnterior) {
        FragmentTransaction transaccion = getSupportFragmentManager().beginTransaction()
                .setReorderingAllowed(true)
                .replace(R.id.contenedorPantallas, destino);
        if (conservarAnterior) {
            transaccion.addToBackStack(null);
        } else {
            getSupportFragmentManager().popBackStack(null, FragmentManager.POP_BACK_STACK_INCLUSIVE);
        }
        transaccion.commit();
    }

    private void volverAInicioSesion() {
        Intent intencion = new Intent(this, InicioSesionActivity.class);
        intencion.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intencion);
        finish();
    }
}
