package com.example.adoptmemovil;

import static com.example.adoptmemovil.utilidades.InterfazUsuarioUtils.cargarFotoMascota;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.example.adoptmemovil.modelo.Mascota;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.MascotaServicios;
import com.example.adoptmemovil.utilidades.FotoMascotaCache;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ConsultarAdopcionFragment extends Fragment {

    private TextView tvNombre, tvEspecie, tvRaza, tvEdad, tvSexo, tvEstatura, tvDescripcion;
    private ImageView imgFotoMascota;
    private Button btnVerVideo;
    private Mascota mascota;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_consultar_adopcion, container, false);

    }

    @Override
    public void onViewCreated(@NonNull View view,
                              @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        tvNombre = view.findViewById(R.id.tvNombre);
        tvEspecie = view.findViewById(R.id.tvEspecie);
        tvRaza = view.findViewById(R.id.tvRaza);
        tvEdad = view.findViewById(R.id.tvEdad);
        tvSexo = view.findViewById(R.id.tvSexo);
        tvEstatura = view.findViewById(R.id.tvEstatura);
        tvDescripcion = view.findViewById(R.id.tvDescripcion);
        imgFotoMascota = view.findViewById(R.id.imgFotoMascota);
        btnVerVideo = view.findViewById(R.id.btnVerVideo);
        Button btnSolicitarAdopcion = view.findViewById(R.id.btnSolicitarAdopcion);

        if (getArguments() != null) {
            Bundle args = getArguments();
            mascota = args.getParcelable("mascota");
            boolean desdeBottomSheet = args != null && args.getBoolean("desdeBottomSheet", false);

            if (desdeBottomSheet) {
                btnSolicitarAdopcion.setVisibility(View.VISIBLE);
            } else {
                btnSolicitarAdopcion.setVisibility(View.GONE);
            }

            if (mascota != null) {
                tvNombre.setText("Nombre: " + mascota.getNombre());
                tvEspecie.setText("Especie: " + mascota.getEspecie());
                tvRaza.setText("Raza: " + mascota.getRaza());
                tvEdad.setText("Edad: " + mascota.getEdad());
                tvSexo.setText("Sexo: " + mascota.getSexo());
                tvEstatura.setText("Estatura: " + mascota.getTamaño());
                tvDescripcion.setText("Descripción: " + mascota.getDescripcion());
                cargarFotoMascota(requireContext(),UsuarioSingleton.getInstancia().getToken(), mascota.getMascotaID(), imgFotoMascota);
            }
        }

        btnVerVideo.setOnClickListener(v -> {
            Bundle bundle = new Bundle();
            bundle.putInt("mascotaId", mascota.getMascotaID());

            VideoMascotaFragment fragment = new VideoMascotaFragment();
            fragment.setArguments(bundle);

            requireActivity().getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.fragment_container, fragment)
                    .addToBackStack(null)
                    .commit();

        });
    }
}
