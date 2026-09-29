package com.adoptme.movil.ui.adopciones;

import android.net.Uri;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.adoptme.movil.R;
import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Evento;
import com.adoptme.movil.datos.repositorios.AdopcionRepositorio;
import com.adoptme.movil.datos.repositorios.ArchivoRepositorio;
import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Ubicacion;
import com.adoptme.movil.ui.comun.VistaModeloBase;

public final class RegistroAdopcionViewModel extends VistaModeloBase {

    private final AdopcionRepositorio adopciones;
    private final ArchivoRepositorio archivos;
    private final MutableLiveData<Uri> foto = new MutableLiveData<>();
    private final MutableLiveData<Uri> video = new MutableLiveData<>();
    private final MutableLiveData<Ubicacion> ubicacion = new MutableLiveData<>();
    private final MutableLiveData<Evento<Boolean>> registrada = new MutableLiveData<>();

    public RegistroAdopcionViewModel(AdopcionRepositorio adopciones, ArchivoRepositorio archivos) {
        this.adopciones = adopciones;
        this.archivos = archivos;
    }

    public LiveData<Uri> getFoto() {
        return foto;
    }

    public LiveData<Uri> getVideo() {
        return video;
    }

    public LiveData<Ubicacion> getUbicacion() {
        return ubicacion;
    }

    public LiveData<Evento<Boolean>> getRegistrada() {
        return registrada;
    }

    public void seleccionarFoto(Uri archivo) {
        foto.setValue(archivo);
    }

    public void seleccionarVideo(Uri archivo) {
        video.setValue(archivo);
    }

    public void seleccionarUbicacion(Ubicacion seleccionada) {
        ubicacion.setValue(seleccionada);
    }

    public void registrar(FormularioMascota formulario) {
        if (!formulario.estaCompleto()) {
            avisar(R.string.error_campos_obligatorios);
            return;
        }
        if (foto.getValue() == null) {
            avisar(R.string.error_foto_obligatoria);
            return;
        }
        if (ubicacion.getValue() == null) {
            avisar(R.string.error_ubicacion_obligatoria);
            return;
        }
        iniciarCarga();
        adopciones.registrar(formulario.aMascota(), ubicacion.getValue(), resultado -> {
            if (!resultado.esExito()) {
                terminarCarga();
                avisar(resultado.getError());
                return;
            }
            subirArchivos(resultado.getValor().getMascotaId());
        });
    }

    private void subirArchivos(int mascotaId) {
        Uri fotoSeleccionada = foto.getValue();
        Uri videoSeleccionado = video.getValue();
        archivos.subirFotoMascota(mascotaId, fotoSeleccionada, avisarSiFalla(() -> {
            if (videoSeleccionado == null) {
                finalizar();
                return;
            }
            archivos.subirVideoMascota(mascotaId, videoSeleccionado, avisarSiFalla(this::finalizar));
        }));
    }

    private AlTerminar<Void> avisarSiFalla(Runnable continuar) {
        return resultado -> {
            if (!resultado.esExito()) {
                String motivo = resultado.getError().getMensaje();
                avisar(R.string.archivo_no_subido, motivo == null ? "" : motivo);
            }
            continuar.run();
        };
    }

    private void finalizar() {
        terminarCarga();
        avisar(R.string.adopcion_registrada);
        registrada.postValue(new Evento<>(true));
    }

    public static final class FormularioMascota {
        private final String nombre;
        private final String especie;
        private final String raza;
        private final String edad;
        private final String sexo;
        private final String tamano;
        private final String descripcion;

        public FormularioMascota(String nombre, String especie, String raza, String edad, String sexo, String tamano, String descripcion) {
            this.nombre = nombre.trim();
            this.especie = especie.trim();
            this.raza = raza.trim();
            this.edad = edad.trim();
            this.sexo = sexo;
            this.tamano = tamano;
            this.descripcion = descripcion.trim();
        }

        boolean estaCompleto() {
            return !nombre.isEmpty() && !especie.isEmpty() && !raza.isEmpty() && !edad.isEmpty()
                    && sexo != null && tamano != null;
        }

        Mascota aMascota() {
            return new Mascota(0, nombre, especie, raza, edad, sexo, tamano, descripcion.isEmpty() ? null : descripcion);
        }
    }
}
