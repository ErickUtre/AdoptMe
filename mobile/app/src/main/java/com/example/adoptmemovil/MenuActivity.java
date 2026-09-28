package com.example.adoptmemovil;

import static com.example.adoptmemovil.utilidades.InterfazUsuarioUtils.cargarFotoPerfil;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

import com.example.adoptmemovil.utilidades.UsuarioSingleton;

public class MenuActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_menu);

        loadFragment(new MapaFragment());

        ImageView iv_foto = findViewById(R.id.iv_perfil);

        findViewById(R.id.btn_home).setOnClickListener(v -> loadFragment(new MapaFragment()));
        findViewById(R.id.btn_registrarMascota).setOnClickListener(v -> loadFragment(new com.example.adoptmemovil.RegistrarAdopcionFragment()));
        findViewById(R.id.btn_mascotas).setOnClickListener(v -> loadFragment(new AdopcionesRegistradasFragment()));
        findViewById(R.id.btn_perfil).setOnClickListener(v -> loadFragment(new ConsultarUsuarioFragment()));

        findViewById(R.id.btn_logout).setOnClickListener(v -> {
            try {
                UsuarioSingleton.getInstancia().cerrarSesion();

                Intent intent = new Intent(MenuActivity.this, LoginActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                startActivity(intent);

                finish();
            } catch (Exception e) {
                Toast.makeText(this, "Error al cerrar sesión", Toast.LENGTH_SHORT).show();
            }
        });

        cargarFotoPerfil(this, UsuarioSingleton.getInstancia().getToken(), iv_foto);
    }

    private void loadFragment(Fragment fragment) {
        getSupportFragmentManager().beginTransaction()
                .replace(R.id.fragment_container, fragment)
                .commit();
    }
}

