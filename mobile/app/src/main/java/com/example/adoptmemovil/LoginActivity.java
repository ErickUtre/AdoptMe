package com.example.adoptmemovil;

import android.app.AlertDialog;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.adoptmemovil.modelo.ResultadoHttp;
import com.example.adoptmemovil.modelo.Ubicacion;
import com.example.adoptmemovil.modelo.request.IniciarSesionRequest;
import com.example.adoptmemovil.modelo.response.IniciarSesionResponse;
import com.example.adoptmemovil.servicios.AccesoServicios;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.utilidades.Encriptador;
import com.example.adoptmemovil.utilidades.HttpCallback;
import com.example.adoptmemovil.utilidades.HttpHelper;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;

import java.io.Console;

import retrofit2.Call;

public class LoginActivity extends AppCompatActivity {
    private static final String TAG = "LoginActivity";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        if (UsuarioSingleton.getInstancia().estaLogueado()) {
            startActivity(new Intent(this, MenuActivity.class));
            finish();
        }

        Button btnLogin = findViewById(R.id.btnLogin);
        btnLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                EditText etCorreo = findViewById(R.id.etCorreo);
                EditText etPassword = findViewById(R.id.etPassword);

                String correo = etCorreo.getText().toString();
                String password = etPassword.getText().toString();

                if (correo.isEmpty() || password.isEmpty()) {
                    Toast.makeText(LoginActivity.this, "¡Completa todos los campos!", Toast.LENGTH_SHORT).show();
                } else {
                    String contrasenaHash = Encriptador.generarHashSHA512(password);
                    iniciarSesion(correo, contrasenaHash);
                }
            }
        });

        TextView tvRegister = findViewById(R.id.tvRegister);
        tvRegister.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(LoginActivity.this, RegistrarUsuarioActivity.class);
                startActivity(intent);
            }
        });
    }

    public void iniciarSesion(String correo, String contrasenaHash) {
        AccesoServicios servicios = ClienteAPI.getAccesoServicios();

        IniciarSesionRequest request = new IniciarSesionRequest(correo, contrasenaHash);

        Call<IniciarSesionResponse> call = servicios.iniciarSesion(request);

        HttpHelper.ejecutarHttp(call, new HttpCallback<IniciarSesionResponse>() {
            @Override
            public void onRespuesta(ResultadoHttp<IniciarSesionResponse> resultado) {
                if (resultado.exito) {
                    IniciarSesionResponse respuesta = resultado.contenido;
                    UsuarioSingleton.getInstancia().iniciarSesion(respuesta.usuario, respuesta.token);

                    Intent intent;
                    if (!respuesta.esAdmin) {
                        intent = new Intent(LoginActivity.this, MenuActivity.class);
                        finish();
                    } else {
                        intent = new Intent(LoginActivity.this, MenuAdminActivity.class);
                        finish();
                    }
                    startActivity(intent);
                } else if (resultado.codigo == 401) {
                    Toast.makeText(LoginActivity.this, "El correo y/o contraseña son incorrectas", Toast.LENGTH_LONG).show();
                    Log.e(TAG, resultado.mensajeError + resultado.codigo);
                } else {
                    Toast.makeText(LoginActivity.this, resultado.mensajeError, Toast.LENGTH_LONG).show();
                    Log.e(TAG, resultado.mensajeError + resultado.codigo);
                }
            }
        });
    }
}
