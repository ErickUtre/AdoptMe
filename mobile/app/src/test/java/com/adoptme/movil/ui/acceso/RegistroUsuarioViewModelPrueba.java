package com.adoptme.movil.ui.acceso;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;

import com.adoptme.movil.R;

import org.junit.Test;

public final class RegistroUsuarioViewModelPrueba {

    private static final String NOMBRE = "Ana López";
    private static final String CORREO = "ana@adoptme.com";
    private static final String CONTRASENA = "Segura123";
    private static final String TELEFONO = "2281234567";

    @Test
    public void aceptaDatosCompletosYValidos() {
        assertNull(RegistroUsuarioViewModel.validar(NOMBRE, CORREO, CONTRASENA, CONTRASENA, TELEFONO));
    }

    @Test
    public void detectaConfirmacionDistinta() {
        assertEquals(Integer.valueOf(R.string.error_confirmacion),
                RegistroUsuarioViewModel.validar(NOMBRE, CORREO, CONTRASENA, "Otra12345", TELEFONO));
    }

    @Test
    public void reportaElPrimerCampoInvalido() {
        assertEquals(Integer.valueOf(R.string.error_nombre),
                RegistroUsuarioViewModel.validar("", "incorrecto", "debil", "", "1"));
    }
}
