package com.adoptme.movil.comun;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

public final class ValidadorPrueba {

    @Test
    public void aceptaNombresConAcentosYEspacios() {
        assertTrue(Validador.esNombreValido("  María   José  Núñez "));
    }

    @Test
    public void rechazaNombresConDigitos() {
        assertFalse(Validador.esNombreValido("Ana 2"));
    }

    @Test
    public void normalizaEspaciosRepetidos() {
        assertEquals("Ana María", Validador.normalizar("  Ana    María "));
    }

    @Test
    public void validaFormatoDeCorreo() {
        assertTrue(Validador.esCorreoValido("persona@adoptme.com"));
        assertFalse(Validador.esCorreoValido("persona@adoptme"));
        assertFalse(Validador.esCorreoValido(null));
    }

    @Test
    public void exigeTelefonoDeDiezDigitos() {
        assertTrue(Validador.esTelefonoValido("2281234567"));
        assertFalse(Validador.esTelefonoValido("228123456"));
        assertFalse(Validador.esTelefonoValido("22812345ab"));
    }

    @Test
    public void exigeContrasenaSegura() {
        assertTrue(Validador.esContrasenaValida("Segura123"));
        assertFalse(Validador.esContrasenaValida("segura123"));
        assertFalse(Validador.esContrasenaValida("Segura1a"));
        assertFalse(Validador.esContrasenaValida("Segu ra123"));
        assertFalse(Validador.esContrasenaValida("Seg12"));
    }
}
