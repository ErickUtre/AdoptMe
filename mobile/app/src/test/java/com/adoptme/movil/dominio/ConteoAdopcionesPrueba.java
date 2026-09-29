package com.adoptme.movil.dominio;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import com.adoptme.movil.dominio.ConteoAdopciones.ConteoMensual;
import com.adoptme.movil.dominio.Modelos.AdopcionResumen;

import org.junit.Test;

import java.time.YearMonth;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

public final class ConteoAdopcionesPrueba {

    @Test
    public void devuelveListaVaciaSinAdopciones() {
        assertTrue(ConteoAdopciones.porMes(Collections.emptyList()).isEmpty());
    }

    @Test
    public void agrupaPorMesYRellenaMesesSinAdopciones() {
        List<ConteoMensual> conteos = ConteoAdopciones.porMes(Arrays.asList(
                new AdopcionResumen(1, "2025-01-15T10:00:00.000Z"),
                new AdopcionResumen(2, "2025-01-20T10:00:00.000Z"),
                new AdopcionResumen(3, "2025-03-02T10:00:00.000Z")));

        assertEquals(3, conteos.size());
        assertEquals(YearMonth.of(2025, 1), conteos.get(0).getMes());
        assertEquals(2, conteos.get(0).getCantidad());
        assertEquals(YearMonth.of(2025, 2), conteos.get(1).getMes());
        assertEquals(0, conteos.get(1).getCantidad());
        assertEquals(1, conteos.get(2).getCantidad());
    }

    @Test
    public void ignoraFechasAusentesOInvalidas() {
        List<ConteoMensual> conteos = ConteoAdopciones.porMes(Arrays.asList(
                new AdopcionResumen(1, null),
                new AdopcionResumen(2, "no es fecha"),
                new AdopcionResumen(3, "2025-06-01T00:00:00Z")));

        assertEquals(1, conteos.size());
        assertEquals(YearMonth.of(2025, 6), conteos.get(0).getMes());
    }
}
