package com.example.adoptmemovil;

import android.content.Intent;
import android.os.Bundle;
import android.widget.LinearLayout;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

public class MenuAdminActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_menuadmin);

        // Cargar fragmento inicial
        loadFragment(new HomeAdminFragment());

        // Configurar botones
        findViewById(R.id.btn_home).setOnClickListener(v -> loadFragment(new HomeAdminFragment()));
        //findViewById(R.id.btn_registrarMascota).setOnClickListener(v -> loadFragment(new RegistrarAdopcionFragment()));
        findViewById(R.id.btn_perfil).setOnClickListener(v -> loadFragment(new ConsultarUsuarioFragment()));

        findViewById(R.id.btn_logout).setOnClickListener(v -> {
            startActivity(new Intent(MenuAdminActivity.this, LoginActivity.class));
        });
    }

    private void loadFragment(Fragment fragment) {
        getSupportFragmentManager().beginTransaction()
                .replace(R.id.fragment_container, fragment)
                .commit();
    }
}

