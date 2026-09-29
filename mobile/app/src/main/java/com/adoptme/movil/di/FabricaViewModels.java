package com.adoptme.movil.di;

import androidx.annotation.NonNull;
import androidx.lifecycle.ViewModel;
import androidx.lifecycle.ViewModelProvider;

import com.adoptme.movil.ui.acceso.InicioSesionViewModel;
import com.adoptme.movil.ui.acceso.RegistroUsuarioViewModel;
import com.adoptme.movil.ui.adopciones.AdopcionesPropiasViewModel;
import com.adoptme.movil.ui.adopciones.DetalleAdopcionViewModel;
import com.adoptme.movil.ui.adopciones.RegistroAdopcionViewModel;
import com.adoptme.movil.ui.adopciones.SolicitudesViewModel;
import com.adoptme.movil.ui.mapa.MapaViewModel;
import com.adoptme.movil.ui.perfil.PerfilViewModel;
import com.adoptme.movil.ui.principal.PrincipalViewModel;
import com.adoptme.movil.ui.reportes.ReporteViewModel;
import com.adoptme.movil.ui.ubicacion.SeleccionUbicacionViewModel;

import java.util.HashMap;
import java.util.Map;
import java.util.function.Supplier;

final class FabricaViewModels implements ViewModelProvider.Factory {

    private final Map<Class<? extends ViewModel>, Supplier<ViewModel>> creadores = new HashMap<>();

    FabricaViewModels(ContenedorDependencias contenedor) {
        creadores.put(InicioSesionViewModel.class, () -> new InicioSesionViewModel(contenedor.getCuentas(), contenedor.getSesion()));
        creadores.put(RegistroUsuarioViewModel.class, () -> new RegistroUsuarioViewModel(contenedor.getCuentas()));
        creadores.put(PrincipalViewModel.class, () -> new PrincipalViewModel(contenedor.getSesion()));
        creadores.put(MapaViewModel.class, () -> new MapaViewModel(contenedor.getMapa(), contenedor.getSesion()));
        creadores.put(DetalleAdopcionViewModel.class, () -> new DetalleAdopcionViewModel(contenedor.getAdopciones(), contenedor.getSesion()));
        creadores.put(AdopcionesPropiasViewModel.class, () -> new AdopcionesPropiasViewModel(contenedor.getAdopciones()));
        creadores.put(SolicitudesViewModel.class, () -> new SolicitudesViewModel(contenedor.getAdopciones()));
        creadores.put(RegistroAdopcionViewModel.class, () -> new RegistroAdopcionViewModel(contenedor.getAdopciones(), contenedor.getArchivos()));
        creadores.put(PerfilViewModel.class, () -> new PerfilViewModel(contenedor.getCuentas(), contenedor.getArchivos(), contenedor.getSesion()));
        creadores.put(ReporteViewModel.class, () -> new ReporteViewModel(contenedor.getAdopciones(), contenedor.getGaleria()));
        creadores.put(SeleccionUbicacionViewModel.class, () -> new SeleccionUbicacionViewModel(contenedor.getGeocodificador()));
    }

    @NonNull
    @Override
    public <T extends ViewModel> T create(@NonNull Class<T> tipo) {
        Supplier<ViewModel> creador = creadores.get(tipo);
        if (creador == null) {
            throw new IllegalArgumentException("ViewModel no registrado: " + tipo.getName());
        }
        return tipo.cast(creador.get());
    }
}
