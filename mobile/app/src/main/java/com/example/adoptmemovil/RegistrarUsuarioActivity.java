package com.example.adoptmemovil;

import android.content.Intent;
import android.location.Address;
import android.location.Geocoder;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Toast;
import android.Manifest;
import android.content.pm.PackageManager;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import androidx.appcompat.app.AppCompatActivity;

import com.example.adoptmemovil.servicios.UsuarioServicios;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.modelo.Acceso;
import com.example.adoptmemovil.modelo.Ubicacion;
import com.example.adoptmemovil.modelo.Usuario;
import com.example.adoptmemovil.utilidades.CallbackPermiso;
import com.example.adoptmemovil.utilidades.PermisoHelper;
import com.example.adoptmemovil.utilidades.Validador;
import com.example.adoptmemovil.utilidades.Encriptador;
import com.example.adoptmemovil.utilidades.HttpCallback;
import com.example.adoptmemovil.utilidades.HttpHelper;
import com.example.adoptmemovil.modelo.ResultadoHttp;
import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import okhttp3.ResponseBody;
import retrofit2.Call;


public class RegistrarUsuarioActivity extends AppCompatActivity {
    private PermisoHelper permisoHelper;
    private static final String TAG = "RegistrarUsuario";
    private Ubicacion ubicacionFinal;
    private EditText etNombre;
    private EditText etCorreo;
    private EditText etPassword;
    private EditText etConfirmPassword;
    private EditText etTelefono;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_registrarusuario);

        permisoHelper = new PermisoHelper(this, new CallbackPermiso() {
            @Override
            public void onPermisoConcedido() {
                obtenerUbicacionDisponible();
            }

            @Override
            public void onPermisoDenegado() {
                Toast.makeText(RegistrarUsuarioActivity.this, "Permiso de ubicación denegado", Toast.LENGTH_SHORT).show();
            }
        });

        etNombre = findViewById(R.id.etNombre);
        etCorreo = findViewById(R.id.etCorreo);
        etPassword = findViewById(R.id.etPassword);
        etConfirmPassword = findViewById(R.id.etConfirmPassword);
        etTelefono = findViewById(R.id.etTelefono);

        ImageButton btnBack = findViewById(R.id.btnBack);
        btnBack.setOnClickListener(view -> finish());

        Button btnRegistrarUbicacion = findViewById(R.id.btnRegistrarUbicacion);
        btnRegistrarUbicacion.setOnClickListener(view -> {
            if (PermisoHelper.tienePermisoUbicacion(this)) {
                obtenerUbicacionDisponible();
            } else {
                permisoHelper.solicitarPermisoUbicacion();
            }
        });

        Button btnRegistrar = findViewById(R.id.btnRegistrar);
        btnRegistrar.setOnClickListener(view -> {
            if (validarCampos()) {
                Acceso acceso = new Acceso();
                acceso.setCorreo(etCorreo.getText().toString().trim());
                acceso.setContrasenaHash(Encriptador.generarHashSHA512(etPassword.getText().toString()));

                Usuario usuario = new Usuario();
                usuario.setNombre(etNombre.getText().toString().trim());
                usuario.setTelefono(etTelefono.getText().toString().trim());
                usuario.setAcceso(acceso);

                if (ubicacionFinal != null) {
                    usuario.setUbicacion(ubicacionFinal);
                }

                registrarUsuario(usuario);
            }
        });
    }

    private boolean validarCampos() {
        String nombre = etNombre.getText().toString().trim();
        String correo = etCorreo.getText().toString().trim();
        String password = etPassword.getText().toString();
        String confirmPassword = etConfirmPassword.getText().toString();
        String telefono = etTelefono.getText().toString().trim();

        if (nombre.isEmpty() || correo.isEmpty() || password.isEmpty() ||
                confirmPassword.isEmpty() || telefono.isEmpty()) {
            mostrarError("Todos los campos son obligatorios");
            return false;
        }

        if (!Validador.validarNombre(nombre)) {
            mostrarError("Ingrese un nombre válido");
            return false;
        }

        if (!Validador.validarCorreo(correo)) {
            mostrarError("Ingrese un correo válido (ejemplo@dominio.com)");
            return false;
        }

        if (!Validador.validarContrasena(password)) {
            mostrarError("Ingrese una contraseña válida");
            return false;
        }

        if (!password.equals(confirmPassword)) {
            mostrarError("Las contraseñas no coinciden");
            return false;
        }

        if (!Validador.validarTelefono(telefono)) {
            mostrarError("El teléfono debe tener 10 dígitos numéricos");
            return false;
        }

        return true;
    }

    private void mostrarError(String mensaje) {
        Toast.makeText(this, mensaje, Toast.LENGTH_SHORT).show();
    }

    private void limpiarCampos() {
        etNombre.getText().clear();
        etCorreo.getText().clear();
        etPassword.getText().clear();
        etConfirmPassword.getText().clear();
        etTelefono.getText().clear();
    }

    private void obtenerUbicacionDisponible() {
        FusedLocationProviderClient fusedLocationClient = LocationServices.getFusedLocationProviderClient(this);

        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED &&
                ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            return;
        }

        fusedLocationClient.getLastLocation()
                .addOnSuccessListener(location -> {
                    if (location != null) {
                        double latitud = location.getLatitude();
                        double longitud = location.getLongitude();

                        Geocoder geocoder = new Geocoder(this, Locale.getDefault());
                        try {
                            List<Address> direcciones = geocoder.getFromLocation(latitud, longitud, 1);
                            assert direcciones != null;
                            if (!direcciones.isEmpty()) {
                                Intent intent = crearIntentMapaRegistro(direcciones, latitud, longitud);
                                mapaLauncher.launch(intent);
                            }
                        } catch (IOException e) {
                            Toast.makeText(this, "No se pudo obtener dirección", Toast.LENGTH_SHORT).show();
                        }
                    } else {
                        Toast.makeText(this, "No se pudo obtener la ubicación", Toast.LENGTH_SHORT).show();
                    }
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(this, "Error al obtener ubicación: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                });
    }

    @NonNull
    private Intent crearIntentMapaRegistro(List<Address> direcciones, double latitud, double longitud) {
        Address direccion = direcciones.get(0);
        String ciudad = direccion.getLocality();
        String estado = direccion.getAdminArea();
        String pais = direccion.getCountryName();

        Intent intent = new Intent(this, MapaRegistroActivity.class);
        intent.putExtra("latitud", latitud);
        intent.putExtra("longitud", longitud);
        intent.putExtra("ciudad", ciudad);
        intent.putExtra("estado", estado);
        intent.putExtra("pais", pais);
        return intent;
    }

    private ActivityResultLauncher<Intent> mapaLauncher = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            result -> {
                if (result.getResultCode() == RESULT_OK && result.getData() != null) {
                    Intent data = result.getData();
                    double latitud = data.getDoubleExtra("latitud", 0.0);
                    double longitud = data.getDoubleExtra("longitud", 0.0);
                    String ciudad = data.getStringExtra("ciudad");
                    String estado = data.getStringExtra("estado");
                    String pais = data.getStringExtra("pais");

                    ubicacionFinal = new Ubicacion();
                    ubicacionFinal.setLatitud(latitud);
                    ubicacionFinal.setLongitud(longitud);
                    ubicacionFinal.setCiudad(ciudad);
                    ubicacionFinal.setEstado(estado);
                    ubicacionFinal.setPais(pais);

                    Toast.makeText(this, "Ubicación registrada correctamente", Toast.LENGTH_SHORT).show();
                } else {
                    Toast.makeText(this, "Registro de ubicación cancelado", Toast.LENGTH_SHORT).show();
                }
            }
    );

    public void registrarUsuario(Usuario usuario) {
        UsuarioServicios servicios = ClienteAPI.getUsuarioServicios();

        Call<ResponseBody> call = servicios.registrarUsuario(usuario);

        HttpHelper.ejecutarHttp(call, new HttpCallback<ResponseBody>() {
            @Override
            public void onRespuesta(ResultadoHttp<ResponseBody> resultado) {
                if (resultado.exito) {
                    Toast.makeText(RegistrarUsuarioActivity.this, "Registro exitoso", Toast.LENGTH_LONG).show();
                    limpiarCampos();
                    finish();
                } else if (resultado.codigo == 409) {
                    Toast.makeText(RegistrarUsuarioActivity.this, "El correo ya esta registrado", Toast.LENGTH_LONG).show();
                } else {
                    Toast.makeText(RegistrarUsuarioActivity.this, resultado.mensajeError, Toast.LENGTH_LONG).show();
                    Log.e(TAG, resultado.mensajeError + resultado.codigo);
                }
            }
        });
    }
}
