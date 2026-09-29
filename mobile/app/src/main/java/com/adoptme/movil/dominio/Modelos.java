package com.adoptme.movil.dominio;

import com.google.gson.annotations.SerializedName;

public final class Modelos {

    private Modelos() {
    }

    public static final class DatosAcceso {
        @SerializedName("Correo")
        private final String correo;
        @SerializedName("EsAdmin")
        private final boolean esAdmin;

        public DatosAcceso(String correo, boolean esAdmin) {
            this.correo = correo;
            this.esAdmin = esAdmin;
        }

        public String getCorreo() {
            return correo;
        }

        public boolean esAdmin() {
            return esAdmin;
        }
    }

    public static final class Perfil {
        @SerializedName("UsuarioID")
        private final int usuarioId;
        @SerializedName("Nombre")
        private final String nombre;
        @SerializedName("Telefono")
        private final String telefono;
        @SerializedName("Ubicacion")
        private final Ubicacion ubicacion;
        @SerializedName("Acceso")
        private final DatosAcceso acceso;

        public Perfil(int usuarioId, String nombre, String telefono, Ubicacion ubicacion, DatosAcceso acceso) {
            this.usuarioId = usuarioId;
            this.nombre = nombre;
            this.telefono = telefono;
            this.ubicacion = ubicacion;
            this.acceso = acceso;
        }

        public int getUsuarioId() {
            return usuarioId;
        }

        public String getNombre() {
            return nombre;
        }

        public String getTelefono() {
            return telefono;
        }

        public Ubicacion getUbicacion() {
            return ubicacion;
        }

        public DatosAcceso getAcceso() {
            return acceso;
        }

        public Perfil conUbicacion(Ubicacion nueva) {
            return new Perfil(usuarioId, nombre, telefono, nueva, acceso);
        }

        public Perfil conCorreo(String correo) {
            return new Perfil(usuarioId, nombre, telefono, ubicacion, new DatosAcceso(correo, acceso.esAdmin()));
        }
    }

    public static final class Sesion {
        @SerializedName("token")
        private final String token;
        @SerializedName("esAdmin")
        private final boolean esAdmin;
        @SerializedName("usuario")
        private final Perfil usuario;

        public Sesion(String token, boolean esAdmin, Perfil usuario) {
            this.token = token;
            this.esAdmin = esAdmin;
            this.usuario = usuario;
        }

        public String getToken() {
            return token;
        }

        public boolean esAdmin() {
            return esAdmin;
        }

        public Perfil getUsuario() {
            return usuario;
        }
    }

    public static final class Adopcion {
        @SerializedName("AdopcionID")
        private final int adopcionId;
        @SerializedName("Estado")
        private final boolean adoptada;
        @SerializedName("MascotaID")
        private final int mascotaId;
        @SerializedName("PublicadorID")
        private final int publicadorId;
        @SerializedName("Mascota")
        private final Mascota mascota;

        public Adopcion(int adopcionId, boolean adoptada, int mascotaId, int publicadorId, Mascota mascota) {
            this.adopcionId = adopcionId;
            this.adoptada = adoptada;
            this.mascotaId = mascotaId;
            this.publicadorId = publicadorId;
            this.mascota = mascota;
        }

        public int getAdopcionId() {
            return adopcionId;
        }

        public boolean estaAdoptada() {
            return adoptada;
        }

        public int getMascotaId() {
            return mascotaId;
        }

        public int getPublicadorId() {
            return publicadorId;
        }

        public Mascota getMascota() {
            return mascota;
        }
    }

    public static final class AdopcionRegistrada {
        @SerializedName("AdopcionID")
        private final int adopcionId;
        @SerializedName("MascotaID")
        private final int mascotaId;

        public AdopcionRegistrada(int adopcionId, int mascotaId) {
            this.adopcionId = adopcionId;
            this.mascotaId = mascotaId;
        }

        public int getAdopcionId() {
            return adopcionId;
        }

        public int getMascotaId() {
            return mascotaId;
        }
    }

    public static final class AdopcionResumen {
        @SerializedName("AdopcionID")
        private final int adopcionId;
        @SerializedName("FechaSolicitud")
        private final String fechaSolicitud;

        public AdopcionResumen(int adopcionId, String fechaSolicitud) {
            this.adopcionId = adopcionId;
            this.fechaSolicitud = fechaSolicitud;
        }

        public int getAdopcionId() {
            return adopcionId;
        }

        public String getFechaSolicitud() {
            return fechaSolicitud;
        }
    }

    public static final class Solicitud {
        @SerializedName("SolicitudID")
        private final int solicitudId;
        @SerializedName("AdopcionID")
        private final int adopcionId;
        @SerializedName("AdoptanteID")
        private final int adoptanteId;
        @SerializedName("NombreAdoptante")
        private final String nombreAdoptante;

        public Solicitud(int solicitudId, int adopcionId, int adoptanteId, String nombreAdoptante) {
            this.solicitudId = solicitudId;
            this.adopcionId = adopcionId;
            this.adoptanteId = adoptanteId;
            this.nombreAdoptante = nombreAdoptante;
        }

        public int getSolicitudId() {
            return solicitudId;
        }

        public int getAdopcionId() {
            return adopcionId;
        }

        public int getAdoptanteId() {
            return adoptanteId;
        }

        public String getNombreAdoptante() {
            return nombreAdoptante;
        }
    }
}
