package com.adoptme.movil.datos.remoto.api;

import com.adoptme.movil.dominio.Mascota;
import com.adoptme.movil.dominio.Modelos.Adopcion;
import com.adoptme.movil.dominio.Modelos.AdopcionRegistrada;
import com.adoptme.movil.dominio.Modelos.AdopcionResumen;
import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Modelos.Sesion;
import com.adoptme.movil.dominio.Modelos.Solicitud;
import com.adoptme.movil.dominio.Ubicacion;
import com.google.gson.annotations.SerializedName;

import java.util.List;
import java.util.Map;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.DELETE;
import retrofit2.http.GET;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Path;
import retrofit2.http.Query;

public final class ApisRest {

    private ApisRest() {
    }

    public interface AccesoApi {
        @POST("acceso/iniciar-sesion")
        Call<Sesion> iniciarSesion(@Body Credenciales credenciales);

        @PATCH("acceso")
        Call<Void> actualizar(@Body Map<String, String> cambios);
    }

    public interface UsuarioApi {
        @POST("usuarios")
        Call<Void> registrar(@Body RegistroUsuario registro);

        @PATCH("usuarios/yo")
        Call<Perfil> actualizarPerfil(@Body Map<String, String> cambios);

        @PUT("usuarios/yo/ubicacion")
        Call<Ubicacion> actualizarUbicacion(@Body Ubicacion ubicacion);
    }

    public interface AdopcionApi {
        @POST("adopciones")
        Call<AdopcionRegistrada> registrar(@Body NuevaAdopcion adopcion);

        @GET("adopciones/propias")
        Call<List<Adopcion>> listarPropias();

        @DELETE("adopciones/{id}")
        Call<Void> eliminar(@Path("id") int adopcionId);

        @GET("adopciones")
        Call<List<AdopcionResumen>> listarParaReporte(@Query("estado") String estado);
    }

    public interface SolicitudApi {
        @GET("adopciones/{adopcionId}/solicitudes")
        Call<List<Solicitud>> listar(@Path("adopcionId") int adopcionId);

        @POST("adopciones/{adopcionId}/solicitudes")
        Call<Void> registrar(@Path("adopcionId") int adopcionId);

        @POST("adopciones/{adopcionId}/solicitudes/{solicitudId}/aceptacion")
        Call<Void> aceptar(@Path("adopcionId") int adopcionId, @Path("solicitudId") int solicitudId);

        @DELETE("adopciones/{adopcionId}/solicitudes/{solicitudId}")
        Call<Void> rechazar(@Path("adopcionId") int adopcionId, @Path("solicitudId") int solicitudId);
    }

    public static final class Credenciales {
        @SerializedName("Correo")
        private final String correo;
        @SerializedName("Contrasena")
        private final String contrasena;

        public Credenciales(String correo, String contrasena) {
            this.correo = correo;
            this.contrasena = contrasena;
        }
    }

    public static final class RegistroUsuario {
        @SerializedName("Nombre")
        private final String nombre;
        @SerializedName("Telefono")
        private final String telefono;
        @SerializedName("Ubicacion")
        private final Ubicacion ubicacion;
        @SerializedName("Acceso")
        private final Credenciales acceso;

        public RegistroUsuario(String nombre, String telefono, Ubicacion ubicacion, Credenciales acceso) {
            this.nombre = nombre;
            this.telefono = telefono;
            this.ubicacion = ubicacion;
            this.acceso = acceso;
        }
    }

    public static final class NuevaAdopcion {
        @SerializedName("Mascota")
        private final Mascota mascota;
        @SerializedName("Ubicacion")
        private final Ubicacion ubicacion;

        public NuevaAdopcion(Mascota mascota, Ubicacion ubicacion) {
            this.mascota = mascota;
            this.ubicacion = ubicacion;
        }
    }
}
