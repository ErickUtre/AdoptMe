package com.example.adoptmemovil.utilidades;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.adoptmemovil.R;
import com.example.adoptmemovil.modelo.Solicitud;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.SolicitudAdopcionServicios;
import com.example.adoptmemovil.servicios.SolicitudServicios;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SolicitudesPendientesAdapter extends RecyclerView.Adapter<SolicitudesPendientesAdapter.ViewHolder> {

    private final List<Solicitud> listaSolicitudes;
    private final Context contexto;

    public SolicitudesPendientesAdapter(List<Solicitud> listaSolicitudes, Context contexto) {
        this.listaSolicitudes = listaSolicitudes;
        this.contexto = contexto;
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View vista = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_solicitud_pendiente, parent, false);
        return new ViewHolder(vista);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        Solicitud solicitud = listaSolicitudes.get(position);
        holder.txtNombreUsuario.setText(solicitud.getNombreUsuarioAdoptante());

        holder.btnAceptar.setOnClickListener(v -> {
            eliminarSolicitud(solicitud.getSolicitudID(), () -> {
                eliminarSolicitudAdopcion(solicitud.getAdopcionID(), () -> {
                    Toast.makeText(contexto, "Solicitud aceptada y adopción eliminada", Toast.LENGTH_SHORT).show();
                    listaSolicitudes.remove(position);
                    notifyItemRemoved(position);
                });
            });
        });

        holder.btnRechazar.setOnClickListener(v -> {
            eliminarSolicitud(solicitud.getSolicitudID(), () -> {
                Toast.makeText(contexto, "Solicitud rechazada", Toast.LENGTH_SHORT).show();
                listaSolicitudes.remove(position);
                notifyItemRemoved(position);
            });
        });
    }

    @Override
    public int getItemCount() {
        return listaSolicitudes.size();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtNombreUsuario;
        Button btnAceptar, btnRechazar;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtNombreUsuario = itemView.findViewById(R.id.txtNombreUsuario);
            btnAceptar = itemView.findViewById(R.id.btnAceptar);
            btnRechazar = itemView.findViewById(R.id.btnRechazar);
        }
    }

    private void eliminarSolicitud(int solicitudID, Runnable onSuccess) {
        SolicitudServicios servicio = ClienteAPI.getRetrofit().create(SolicitudServicios.class);
        Call<ResponseBody> call = servicio.eliminarSolicitud(solicitudID);
        call.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    onSuccess.run();
                } else {
                    Toast.makeText(contexto, "Error al eliminar la solicitud", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(contexto, "Fallo en la red al eliminar solicitud", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void eliminarSolicitudAdopcion(int solicitudAdopcionID, Runnable onSuccess) {
        SolicitudAdopcionServicios servicio = ClienteAPI.getRetrofit().create(SolicitudAdopcionServicios.class);
        Call<ResponseBody> call = servicio.eliminarSolicitudAdopcion(solicitudAdopcionID);
        call.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful()) {
                    onSuccess.run();
                } else {
                    Toast.makeText(contexto, "Error al eliminar la solicitud de adopción", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(contexto, "Fallo en la red al eliminar solicitud de adopción", Toast.LENGTH_SHORT).show();
            }
        });
    }
}
