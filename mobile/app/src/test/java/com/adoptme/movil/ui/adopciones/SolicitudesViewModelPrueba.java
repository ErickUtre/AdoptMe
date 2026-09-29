package com.adoptme.movil.ui.adopciones;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;

import androidx.arch.core.executor.testing.InstantTaskExecutorRule;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.ui.comun.Aviso;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;

public final class SolicitudesViewModelPrueba {

    private static final int ADOPCION = 7;

    @Rule
    public final InstantTaskExecutorRule ejecucionInmediata = new InstantTaskExecutorRule();

    private AdopcionRepositorioFalso repositorio;
    private SolicitudesViewModel vistaModelo;

    @Before
    public void preparar() {
        repositorio = new AdopcionRepositorioFalso();
        repositorio.solicitudes.add(new Solicitud(1, ADOPCION, 10, "Ana"));
        repositorio.solicitudes.add(new Solicitud(2, ADOPCION, 11, "Luis"));
        vistaModelo = new SolicitudesViewModel(repositorio);
    }

    @Test
    public void cargaLasSolicitudesPendientes() {
        vistaModelo.cargar(ADOPCION);

        assertEquals(2, vistaModelo.getPendientes().getValue().size());
        assertFalse(vistaModelo.getCargando().getValue());
    }

    @Test
    public void rechazarQuitaLaSolicitudDeLaLista() {
        vistaModelo.cargar(ADOPCION);

        vistaModelo.rechazar(repositorio.solicitudes.get(0));

        assertEquals(1, vistaModelo.getPendientes().getValue().size());
        assertEquals(2, vistaModelo.getPendientes().getValue().get(0).getSolicitudId());
        assertEquals(R.string.solicitud_rechazada, vistaModelo.getAvisos().getValue().consumir().getRecurso());
    }

    @Test
    public void aceptarNotificaLaAdopcion() {
        vistaModelo.cargar(ADOPCION);

        vistaModelo.aceptar(repositorio.solicitudes.get(1));

        assertEquals(Integer.valueOf(2), repositorio.aceptadas.get(0));
        assertTrue(vistaModelo.getAdopcionAceptada().getValue().consumir());
    }

    @Test
    public void informaElErrorCuandoFallaLaCarga() {
        repositorio.fallar = true;

        vistaModelo.cargar(ADOPCION);

        Aviso aviso = vistaModelo.getAvisos().getValue().consumir();
        assertEquals(TipoError.CONEXION, aviso.getError().getTipo());
        assertTrue(vistaModelo.getPendientes().getValue().isEmpty());
        assertNull(vistaModelo.getAdopcionAceptada().getValue());
    }
}
