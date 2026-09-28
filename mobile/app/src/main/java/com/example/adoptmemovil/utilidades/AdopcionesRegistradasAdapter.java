package com.example.adoptmemovil.utilidades;

import static com.example.adoptmemovil.utilidades.InterfazUsuarioUtils.cargarFotoMascota;

import android.content.Context;
import android.graphics.Color;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.FragmentActivity;
import androidx.recyclerview.widget.RecyclerView;

import com.example.adoptmemovil.ConsultarAdopcionFragment;
import com.example.adoptmemovil.R;
import com.example.adoptmemovil.modelo.Mascota;
import com.example.adoptmemovil.modelo.SolicitudAdopcion;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.SolicitudAdopcionServicios;

import java.util.List;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class AdopcionesRegistradasAdapter extends RecyclerView.Adapter<AdopcionesRegistradasAdapter.ViewHolder> {

    private Context context;
    private List<SolicitudAdopcion> listaSolicitudes;
    private OnEliminarSolicitudListener eliminarListener;


    public AdopcionesRegistradasAdapter(Context context, List<SolicitudAdopcion> listaSolicitudes) {
        this.context = context;
        this.listaSolicitudes = listaSolicitudes;
    }

    public interface OnEliminarSolicitudListener {
        void onEliminarSolicitud(int solicitudId);
    }

    public void setOnEliminarSolicitudListener(OnEliminarSolicitudListener listener) {
        this.eliminarListener = listener;
    }


    @NonNull
    @Override
    public AdopcionesRegistradasAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_adopcion_registrada, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull AdopcionesRegistradasAdapter.ViewHolder holder, int position) {
        SolicitudAdopcion solicitudAdopcion = listaSolicitudes.get(position);
        Mascota mascota = solicitudAdopcion.getMascota();

        if (mascota != null) {
            holder.txtNombreMascota.setText(mascota.getNombre());

            String especieRazaEdad = String.format("%s · %s · %s",
                    mascota.getEspecie() != null ? mascota.getEspecie() : "N/D",
                    mascota.getRaza() != null ? mascota.getRaza() : "N/D",
                    mascota.getEdad() != null ? mascota.getEdad() : "N/D");
            holder.txtEspecieRazaEdad.setText(especieRazaEdad);

            if (solicitudAdopcion.isEstado()) {
                holder.txtEstadoSolicitud.setText("Estado: Adoptado");
                holder.txtEstadoSolicitud.setTextColor(Color.parseColor("#BB0000"));
            } else {
                holder.txtEstadoSolicitud.setText("Estado: Disponible");
                holder.txtEstadoSolicitud.setTextColor(Color.parseColor("#007700"));
            }

            cargarFotoMascota(context, UsuarioSingleton.getInstancia().getToken(), mascota.getMascotaID(), holder.imgFotoMascota);
        } else {
            holder.txtNombreMascota.setText("N/D");
            holder.txtEspecieRazaEdad.setText("N/D · N/D · N/D");
            holder.txtEstadoSolicitud.setText("Estado: N/D");
            holder.imgFotoMascota.setImageResource(R.drawable.defaultpet);
        }

        holder.btnEliminar.setOnClickListener(v -> {
            if (eliminarListener != null) {
                eliminarListener.onEliminarSolicitud(solicitudAdopcion.getSolicitudAdopcionID());
            }
        });

        holder.btnSolicitudesPendientes.setOnClickListener(v -> {
            if (context instanceof FragmentActivity) {
                FragmentActivity activity = (FragmentActivity) context;

                int adopcionID = solicitudAdopcion.getSolicitudAdopcionID(); // Asegúrate de tener el getter correcto

                // Llama al fragmento y le pasa la adopcionID
                SolicitudesPendientesDialogFragment dialog = SolicitudesPendientesDialogFragment.newInstance(adopcionID);
                dialog.show(activity.getSupportFragmentManager(), "SolicitudesPendientesDialog");
            }
        });

        holder.btnDetallesMascota.setOnClickListener(v -> {
            if (context instanceof FragmentActivity) {
                FragmentActivity activity = (FragmentActivity) context;
                Bundle bundle = new Bundle();
                bundle.putParcelable("mascota", solicitudAdopcion.getMascota());

                ConsultarAdopcionFragment fragment = new ConsultarAdopcionFragment();
                fragment.setArguments(bundle);

                activity.getSupportFragmentManager()
                        .beginTransaction()
                        .replace(R.id.fragment_container, fragment)
                        .addToBackStack(null)
                        .commit();
            }
        });

    }

    @Override
    public int getItemCount() {
        return listaSolicitudes != null ? listaSolicitudes.size() : 0;
    }

    public static class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtNombreMascota, txtEspecieRazaEdad, txtEstadoSolicitud;
        ImageView imgFotoMascota;
        Button btnEliminar, btnSolicitudesPendientes, btnDetallesMascota;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtNombreMascota = itemView.findViewById(R.id.txtNombreMascota);
            txtEspecieRazaEdad = itemView.findViewById(R.id.txtEspecieRazaEdad);
            txtEstadoSolicitud = itemView.findViewById(R.id.txtEstadoSolicitud);
            imgFotoMascota = itemView.findViewById(R.id.imgFotoMascota);
            btnEliminar = itemView.findViewById(R.id.btnEliminar);
            btnSolicitudesPendientes = itemView.findViewById(R.id.btnSolicitudesPendientes);
            btnDetallesMascota = itemView.findViewById(R.id.btnDetallesMascota);
        }
    }
}
