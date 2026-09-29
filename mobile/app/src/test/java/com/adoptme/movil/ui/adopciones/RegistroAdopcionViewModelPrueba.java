package com.adoptme.movil.ui.adopciones;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;

import androidx.arch.core.executor.testing.InstantTaskExecutorRule;

import com.adoptme.movil.R;
import com.adoptme.movil.ui.adopciones.RegistroAdopcionViewModel.FormularioMascota;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;

public final class RegistroAdopcionViewModelPrueba {

    @Rule
    public final InstantTaskExecutorRule ejecucionInmediata = new InstantTaskExecutorRule();

    private AdopcionRepositorioFalso repositorio;
    private RegistroAdopcionViewModel vistaModelo;

    @Before
    public void preparar() {
        repositorio = new AdopcionRepositorioFalso();
        vistaModelo = new RegistroAdopcionViewModel(repositorio, null);
    }

    @Test
    public void exigeLosCamposObligatorios() {
        vistaModelo.registrar(new FormularioMascota("", "Perro", "Mestizo", "1 año(s) con 0 mes(es)", "Macho", "Mediano", ""));

        assertEquals(R.string.error_campos_obligatorios, vistaModelo.getAvisos().getValue().consumir().getRecurso());
        assertNull(repositorio.mascotaRegistrada);
    }

    @Test
    public void exigeUnaFotoAntesDeRegistrar() {
        vistaModelo.registrar(new FormularioMascota("Fido", "Perro", "Mestizo", "1 año(s) con 0 mes(es)", "Macho", "Mediano", ""));

        assertEquals(R.string.error_foto_obligatoria, vistaModelo.getAvisos().getValue().consumir().getRecurso());
        assertNull(repositorio.mascotaRegistrada);
    }
}
