package com.adoptme.movil.ui.adopciones;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.datos.sesion.SesionUsuario;
import com.adoptme.movil.ui.comun.VistaModeloBase;

public final class DetalleAdopcionViewModel extends VistaModeloBase {

    private final AdopcionRepositorio adopciones;
    private final SesionUsuario sesion;

    public DetalleAdopcionViewModel(AdopcionRepositorio adopciones, SesionUsuario sesion) {
        this.adopciones = adopciones;
        this.sesion = sesion;
    }

    public boolean puedeSolicitar(int publicadorId) {
        return !sesion.esAdministrador() && publicadorId != sesion.getUsuarioId();
    }

    public void solicitar(int adopcionId) {
        iniciarCarga();
        adopciones.solicitar(adopcionId, resultado -> {
            terminarCarga();
            if (resultado.esExito()) {
                avisar(R.string.solicitud_enviada);
            } else if (resultado.getError().getTipo() == TipoError.CONFLICTO) {
                avisar(R.string.solicitud_duplicada);
            } else {
                avisar(resultado.getError());
            }
        });
    }
}
