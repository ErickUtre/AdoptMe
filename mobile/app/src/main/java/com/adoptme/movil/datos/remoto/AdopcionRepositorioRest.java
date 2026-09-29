package com.adoptme.movil.datos.remoto;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.datos.remoto.api.ApisRest.AdopcionApi;
import com.adoptme.movil.datos.remoto.api.ApisRest.NuevaAdopcion;
import com.adoptme.movil.datos.remoto.api.ApisRest.SolicitudApi;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.dominio.Modelos.AdopcionRegistrada;
import com.adoptme.movil.dominio.Modelos.AdopcionResumen;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.dominio.Ubicacion;

import java.util.List;

public final class AdopcionRepositorioRest implements AdopcionRepositorio {

    private final AdopcionApi adopciones;
    private final SolicitudApi solicitudes;

    public AdopcionRepositorioRest(ClienteRest cliente) {
        adopciones = cliente.crear(AdopcionApi.class);
        solicitudes = cliente.crear(SolicitudApi.class);
    }

    @Override
    public void registrar(Mascota mascota, Ubicacion ubicacion, AlTerminar<AdopcionRegistrada> alTerminar) {
        LlamadasApi.ejecutar(adopciones.registrar(new NuevaAdopcion(mascota, ubicacion)), alTerminar);
    }

    @Override
    public void listarPropias(AlTerminar<List<Adopcion>> alTerminar) {
        LlamadasApi.ejecutar(adopciones.listarPropias(), alTerminar);
    }

    @Override
    public void eliminar(int adopcionId, AlTerminar<Void> alTerminar) {
        LlamadasApi.ejecutar(adopciones.eliminar(adopcionId), alTerminar);
    }

    @Override
    public void listarParaReporte(EstadoAdopcion estado, AlTerminar<List<AdopcionResumen>> alTerminar) {
        LlamadasApi.ejecutar(adopciones.listarParaReporte(estado.getFiltro()), alTerminar);
    }

    @Override
    public void listarSolicitudes(int adopcionId, AlTerminar<List<Solicitud>> alTerminar) {
        LlamadasApi.ejecutar(solicitudes.listar(adopcionId), alTerminar);
    }

    @Override
    public void solicitar(int adopcionId, AlTerminar<Void> alTerminar) {
        LlamadasApi.ejecutar(solicitudes.registrar(adopcionId), alTerminar);
    }

    @Override
    public void aceptarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar) {
        LlamadasApi.ejecutar(solicitudes.aceptar(adopcionId, solicitudId), alTerminar);
    }

    @Override
    public void rechazarSolicitud(int adopcionId, int solicitudId, AlTerminar<Void> alTerminar) {
        LlamadasApi.ejecutar(solicitudes.rechazar(adopcionId, solicitudId), alTerminar);
    }
}
