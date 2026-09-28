package com.example.adoptmemovil;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.adoptmemovil.modelo.SolicitudAdopcion;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.SolicitudAdopcionServicios;
import com.example.adoptmemovil.utilidades.AdopcionesRegistradasAdapter;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

import java.util.ArrayList;
import java.util.List;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class AdopcionesRegistradasFragment extends Fragment {

    private EditText inputNombre;
    private RecyclerView recyclerAdopciones;
    private AdopcionesRegistradasAdapter adapter;
    private List<SolicitudAdopcion> listaSolicitudes;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_adopciones_registradas, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view,
                              @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        inputNombre = view.findViewById(R.id.input_nombre);

        recyclerAdopciones = view.findViewById(R.id.recyclerAdopciones);
        recyclerAdopciones.setLayoutManager(new LinearLayoutManager(getContext()));

        listaSolicitudes = new ArrayList<>();
        adapter = new AdopcionesRegistradasAdapter(getContext(), listaSolicitudes);
        adapter.setOnEliminarSolicitudListener(solicitudId -> eliminarSolicitud(solicitudId));
        recyclerAdopciones.setAdapter(adapter);

        cargarSolicitudes();
    }

    private void cargarSolicitudes() {
        int usuarioId = UsuarioSingleton.getInstancia().getUsuarioActual().getUsuarioID();

        SolicitudAdopcionServicios service = ClienteAPI.getRetrofit().create(SolicitudAdopcionServicios.class);
        Call<List<SolicitudAdopcion>> call = service.listarSolicitudesPorUsuario(usuarioId);

        call.enqueue(new Callback<List<SolicitudAdopcion>>() {
            @Override
            public void onResponse(Call<List<SolicitudAdopcion>> call, Response<List<SolicitudAdopcion>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    listaSolicitudes.clear();
                    listaSolicitudes.addAll(response.body());
                    adapter.notifyDataSetChanged();
                } else {
                    Toast.makeText(getContext(), "No se encontraron solicitudes", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<List<SolicitudAdopcion>> call, Throwable t) {
                Toast.makeText(getContext(), "Error al cargar solicitudes: " + t.getMessage(), Toast.LENGTH_LONG).show();
            }
        });
    }

    public void eliminarSolicitud(int solicitudId) {
        SolicitudAdopcionServicios service = ClienteAPI.getRetrofit().create(SolicitudAdopcionServicios.class);
        Call<ResponseBody> call = service.eliminarSolicitudAdopcion(solicitudId);

        call.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(getContext(), "Solicitud eliminada", Toast.LENGTH_SHORT).show();
                    cargarSolicitudes();
                } else {
                    Toast.makeText(getContext(), "Error al eliminar solicitud", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(getContext(), "Error en la conexión: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
