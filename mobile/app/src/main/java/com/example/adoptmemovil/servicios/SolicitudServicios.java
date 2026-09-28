package com.example.adoptmemovil.servicios;

import com.example.adoptmemovil.modelo.Solicitud;

import java.util.List;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.DELETE;
import retrofit2.http.GET;
import retrofit2.http.Path;
import retrofit2.http.POST;
import retrofit2.http.Body;

public interface SolicitudServicios {

    @GET("solicitudes/adoptante/{adopcionID}")
    Call<List<Solicitud>> obtenerSolicitudesConNombresPorAdopcionID(@Path("adopcionID") int adopcionID);

    @DELETE("solicitudes/{id}")
    Call<ResponseBody> eliminarSolicitud(@Path("id") int solicitudID);
}
