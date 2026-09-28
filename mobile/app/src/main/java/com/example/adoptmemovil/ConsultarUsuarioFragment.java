package com.example.adoptmemovil;

import static android.app.Activity.RESULT_OK;
import static android.view.View.VISIBLE;

import static com.example.adoptmemovil.utilidades.Constantes.TIPO_SUBIDA_FOTO_USUARIO;
import static com.example.adoptmemovil.utilidades.InterfazUsuarioUtils.cargarFotoPerfil;

import android.Manifest;
import android.app.AlertDialog;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.drawable.Drawable;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.location.Address;
import android.location.Geocoder;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.InputType;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.fragment.app.Fragment;

import com.example.adoptmemovil.gRPC.ServicioMultimediaGrpcCliente;
import com.example.adoptmemovil.modelo.ResultadoHttp;
import com.example.adoptmemovil.modelo.Ubicacion;
import com.example.adoptmemovil.modelo.Usuario;
import com.example.adoptmemovil.modelo.Acceso;
import com.example.adoptmemovil.servicios.AccesoServicios;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.UbicacionServicios;
import com.example.adoptmemovil.servicios.UsuarioServicios;
import com.example.adoptmemovil.utilidades.CallbackGeneral;
import com.example.adoptmemovil.utilidades.CallbackPermiso;
import com.example.adoptmemovil.utilidades.HttpCallback;
import com.example.adoptmemovil.utilidades.HttpHelper;
import com.example.adoptmemovil.utilidades.MetodoSubida;
import com.example.adoptmemovil.utilidades.PermisoHelper;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;
import com.example.adoptmemovil.utilidades.Validador;
import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import io.grpc.stub.StreamObserver;
import multimedia.ChunkArchivo;
import multimedia.RespuestaGeneral;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ConsultarUsuarioFragment extends Fragment {
    private PermisoHelper permisoHelper;
    private TextView textNombre, textCorreo, textTelefono, textCiudad, textEstado, textPais, textLongitud, textLatitud;
    private ImageButton btnEditarNombre, btnEditarCorreo, btnEditarTelefono, btnEditarFoto;
    private Button btnRegistrarUbicacion;
    private ImageView imageViewPreview;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_consultar_usuario, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view,
                              @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        permisoHelper = new PermisoHelper(this, new CallbackPermiso() {
            @Override
            public void onPermisoConcedido() {
                obtenerUbicacion();
            }

            @Override
            public void onPermisoDenegado() {
                Toast.makeText(requireContext(), "Permiso de ubicación denegado", Toast.LENGTH_SHORT).show();
            }
        });

        textNombre = view.findViewById(R.id.txtNombre);
        textCorreo = view.findViewById(R.id.txtCorreo);
        textTelefono = view.findViewById(R.id.txtTelefono);
        textCiudad = view.findViewById(R.id.txtCiudad);
        textEstado = view.findViewById(R.id.txtEstado);
        textPais = view.findViewById(R.id.txtPais);
        textLatitud = view.findViewById(R.id.txtLatitud);
        textLongitud = view.findViewById(R.id.txtLongitud);

        btnEditarNombre = view.findViewById(R.id.btnEditarNombre);
        btnEditarCorreo = view.findViewById(R.id.btnEditarCorreo);
        btnEditarTelefono = view.findViewById(R.id.btnEditarTelefono);
        btnRegistrarUbicacion = view.findViewById(R.id.btnRegistrarUbicacion);
        btnEditarFoto = view.findViewById(R.id.btnEditarFoto);

        imageViewPreview = view.findViewById(R.id.imageViewPreview);

        imageViewPreview.post(() -> {
            cargarFotoPerfil(requireContext(), UsuarioSingleton.getInstancia().getToken(), imageViewPreview);
        });

        Usuario usuarioActual = UsuarioSingleton.getInstancia().getUsuarioActual();
        String nombre = usuarioActual.getNombre();
        String correo = usuarioActual.getAcceso().getCorreo();
        String telefono = usuarioActual.getTelefono();

        textNombre.setText("Nombre: " + nombre);
        textCorreo.setText("Correo: " + correo);
        textTelefono.setText("Teléfono: " + telefono);

        if (usuarioActual.getUbicacion() != null) {
            String ciudad = usuarioActual.getUbicacion().getCiudad();
            String estado = usuarioActual.getUbicacion().getEstado();
            String pais = usuarioActual.getUbicacion().getPais();
            double latitud = usuarioActual.getUbicacion().getLatitud();
            double longitud = usuarioActual.getUbicacion().getLongitud();

            textCiudad.setText("Ciudad: " + ciudad);
            textEstado.setText("Estado: " + estado);
            textPais.setText("Pais: " + pais);
            textLatitud.setText("Latitud: " + latitud);
            textLongitud.setText("Longitud: " + longitud);

            mostrarDatosUbicacion();
        }

        Drawable avatar = getResources().getDrawable(R.drawable.ic_perfil_defecto);
        imageViewPreview.setImageDrawable(avatar);

        btnEditarNombre.setOnClickListener(v -> {
            mostrarDialogoEditarNombre(usuarioActual.getNombre());
        });

        btnEditarCorreo.setOnClickListener(v -> {
            mostrarDialogoEditarCorreo(usuarioActual.getAcceso().getCorreo());
        });

        btnEditarTelefono.setOnClickListener(v -> {
            mostrarDialogoEditarTelefono(usuarioActual.getTelefono());
        });

        btnRegistrarUbicacion.setOnClickListener(v -> {
            if (PermisoHelper.tienePermisoUbicacion(requireContext())) {
                obtenerUbicacion();
            } else {
                permisoHelper.solicitarPermisoUbicacion();
            }
        });

        btnEditarFoto.setOnClickListener(v -> {
            seleccionarFoto();
        });
    }

    private void mostrarDatosUbicacion() {
        textCiudad.setVisibility(VISIBLE);
        textEstado.setVisibility(VISIBLE);
        textPais.setVisibility(VISIBLE);
        textLatitud.setVisibility(VISIBLE);
        textLongitud.setVisibility(VISIBLE);
    }

    private void mostrarDialogoEditarNombre(String nombreActual) {
        AlertDialog.Builder builder = new AlertDialog.Builder(requireContext());
        builder.setTitle("Editar Nombre");

        final EditText input = new EditText(requireContext());
        input.setInputType(InputType.TYPE_CLASS_TEXT);
        input.setText(nombreActual);
        builder.setView(input);

        builder.setPositiveButton("Guardar", null);
        builder.setNegativeButton("Cancelar", (dialog, which) -> dialog.cancel());

        AlertDialog dialog = builder.create();
        dialog.setOnShowListener(d -> {
            Button btn = dialog.getButton(AlertDialog.BUTTON_POSITIVE);
            btn.setOnClickListener(v -> {
                String nuevoNombre = input.getText().toString().trim();
                if (!Validador.validarNombre(nuevoNombre)) {
                    input.setError("Nombre inválido.");
                    return;
                }

                if (UsuarioSingleton.getInstancia().getUsuarioActual().getNombre().equals(nuevoNombre)) {
                    input.setError("Es su nombre actual.");
                    return;
                }

                actualizarNombre(nuevoNombre, new CallbackGeneral() {
                    @Override
                    public void onExito() {
                        UsuarioSingleton.getInstancia().getUsuarioActual().setNombre(nuevoNombre);
                        textNombre.setText("Nombre: " + nuevoNombre);
                        dialog.dismiss();
                        Toast.makeText(requireContext(), "Nombre actualizado correctamente", Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onError(String mensaje) {
                        Toast.makeText(requireContext(), mensaje, Toast.LENGTH_SHORT).show();
                    }
                });
            });
        });

        dialog.show();
    }

    private void actualizarNombre(String nuevoNombre, CallbackGeneral callback) {
        Usuario usuario = new Usuario();
        usuario.setNombre(nuevoNombre);

        String token = UsuarioSingleton.getInstancia().getToken();

        UsuarioServicios servicio = ClienteAPI.getRetrofit().create(UsuarioServicios.class);
        Call<ResponseBody> llamada = servicio.actualizarPerfil(usuario, "Bearer " + token);

        HttpHelper.ejecutarHttp(llamada, new HttpCallback<ResponseBody>() {
            @Override
            public void onRespuesta(ResultadoHttp<ResponseBody> resultado) {
                if (resultado.exito) {
                    callback.onExito();
                } else {
                    callback.onError("Error al actualizar nombre");
                }
            }
        });
    }

    private void mostrarDialogoEditarCorreo(String correoActual) {
        AlertDialog.Builder builder = new AlertDialog.Builder(requireContext());
        builder.setTitle("Editar Correo");

        final EditText input = new EditText(requireContext());
        input.setInputType(InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS);
        input.setText(correoActual);
        builder.setView(input);

        builder.setPositiveButton("Guardar", null);
        builder.setNegativeButton("Cancelar", (dialog, which) -> dialog.cancel());

        AlertDialog dialog = builder.create();
        dialog.setOnShowListener(d -> {
            Button btn = dialog.getButton(AlertDialog.BUTTON_POSITIVE);
            btn.setOnClickListener(v -> {
                String nuevoCorreo = input.getText().toString().trim();
                if (!Validador.validarCorreo(nuevoCorreo)) {
                    input.setError("Correo inválido.");
                    return;
                }

                if (UsuarioSingleton.getInstancia().getUsuarioActual().getAcceso().getCorreo().equals(nuevoCorreo)) {
                    input.setError("Es su correo actual.");
                    return;
                }

                actualizarCorreo(nuevoCorreo, new CallbackGeneral() {
                    @Override
                    public void onExito() {
                        UsuarioSingleton.getInstancia().getUsuarioActual().getAcceso().setCorreo(nuevoCorreo);
                        textCorreo.setText("Correo: " + nuevoCorreo);
                        dialog.dismiss();
                        Toast.makeText(requireContext(), "Correo actualizado correctamente", Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onError(String mensaje) {
                        Toast.makeText(requireContext(), mensaje, Toast.LENGTH_SHORT).show();
                    }
                });
            });
        });

        dialog.show();
    }

    private void actualizarCorreo(String nuevoCorreo, CallbackGeneral callback) {
        Acceso acceso = new Acceso();
        acceso.setCorreo(nuevoCorreo);

        String token = UsuarioSingleton.getInstancia().getToken();

        AccesoServicios servicio = ClienteAPI.getRetrofit().create(AccesoServicios.class);
        Call<ResponseBody> llamada = servicio.actualizarAcceso(acceso, "Bearer " + token);

        HttpHelper.ejecutarHttp(llamada, new HttpCallback<ResponseBody>() {
            @Override
            public void onRespuesta(ResultadoHttp<ResponseBody> resultado) {
                if (resultado.exito) {
                    callback.onExito();
                } else {
                    callback.onError("Error al actualizar correo");
                }
            }
        });
    }

    private void mostrarDialogoEditarTelefono(String telefonoActual) {
        AlertDialog.Builder builder = new AlertDialog.Builder(requireContext());
        builder.setTitle("Editar Teléfono");

        final EditText input = new EditText(requireContext());
        input.setInputType(InputType.TYPE_CLASS_PHONE);
        input.setText(telefonoActual);
        builder.setView(input);

        builder.setPositiveButton("Guardar", null);
        builder.setNegativeButton("Cancelar", (dialog, which) -> dialog.cancel());

        AlertDialog dialog = builder.create();
        dialog.setOnShowListener(d -> {
            Button btn = dialog.getButton(AlertDialog.BUTTON_POSITIVE);
            btn.setOnClickListener(v -> {
                String nuevoTelefono = input.getText().toString().trim();
                if (!Validador.validarTelefono(nuevoTelefono)) {
                    input.setError("Telefono inválido.");
                    return;
                }

                if (UsuarioSingleton.getInstancia().getUsuarioActual().getTelefono().equals(nuevoTelefono)) {
                    input.setError("Es su telefono actual.");
                    return;
                }

                actualizarTelefono(nuevoTelefono, new CallbackGeneral() {
                    @Override
                    public void onExito() {
                        UsuarioSingleton.getInstancia().getUsuarioActual().setTelefono(nuevoTelefono);
                        textTelefono.setText("Teléfono: " + nuevoTelefono);
                        dialog.dismiss();
                        Toast.makeText(requireContext(), "Teléfono actualizado correctamente", Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onError(String mensaje) {
                        Toast.makeText(requireContext(), mensaje, Toast.LENGTH_SHORT).show();
                    }
                });
            });
        });

        dialog.show();
    }

    private void actualizarTelefono(String nuevoTelefono, CallbackGeneral callback) {
        Usuario usuario = new Usuario();
        usuario.setTelefono(nuevoTelefono);

        String token = UsuarioSingleton.getInstancia().getToken();

        UsuarioServicios servicio = ClienteAPI.getRetrofit().create(UsuarioServicios.class);
        Call<ResponseBody> llamada = servicio.actualizarPerfil(usuario, "Bearer " + token);

        HttpHelper.ejecutarHttp(llamada, new HttpCallback<ResponseBody>() {
            @Override
            public void onRespuesta(ResultadoHttp<ResponseBody> resultado) {
                if (resultado.exito) {
                    callback.onExito();
                } else {
                    callback.onError("Error al actualizar teléfono");
                }
            }
        });
    }

    private void obtenerUbicacion() {
        FusedLocationProviderClient fusedLocationClient =
                LocationServices.getFusedLocationProviderClient(requireContext());

        if (ActivityCompat.checkSelfPermission(requireContext(), Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED &&
                ActivityCompat.checkSelfPermission(requireContext(), Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            return;
        }

        fusedLocationClient.getLastLocation()
                .addOnSuccessListener(location -> {
                    if (location != null) {
                        double latitud = location.getLatitude();
                        double longitud = location.getLongitude();
                        Geocoder geocoder = new Geocoder(requireContext(), Locale.getDefault());
                        try {
                            List<Address> direcciones = geocoder.getFromLocation(latitud, longitud, 1);
                            assert direcciones != null;
                            if (!direcciones.isEmpty()) {
                                Intent intent = crearIntentMapaRegistro(direcciones, latitud, longitud);
                                mapaLauncher.launch(intent);
                            }
                        } catch (IOException e) {
                            Toast.makeText(requireContext(), "No se pudo obtener dirección", Toast.LENGTH_SHORT).show();
                        }
                    }
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(requireContext(), "Error al obtener ubicación", Toast.LENGTH_SHORT).show();
                });
    }

    @NonNull
    private Intent crearIntentMapaRegistro(List<Address> direcciones, double latitud, double longitud) {
        Address direccion = direcciones.get(0);
        String ciudad = direccion.getLocality();
        String estado = direccion.getAdminArea();
        String pais = direccion.getCountryName();

        Intent intent = new Intent(requireContext(), MapaRegistroActivity.class);
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

                    Ubicacion nuevaUbicacion = new Ubicacion();
                    nuevaUbicacion.setLatitud(latitud);
                    nuevaUbicacion.setLongitud(longitud);
                    nuevaUbicacion.setCiudad(ciudad);
                    nuevaUbicacion.setEstado(estado);
                    nuevaUbicacion.setPais(pais);

                    registrarUbicacion(nuevaUbicacion, new CallbackGeneral() {
                        @Override
                        public void onExito() {
                            UsuarioSingleton.getInstancia().getUsuarioActual().setUbicacion(nuevaUbicacion);
                            textCiudad.setText("Ciudad: " + nuevaUbicacion.getCiudad());
                            textEstado.setText("Estado: " + nuevaUbicacion.getEstado());
                            textPais.setText("Pais: " + nuevaUbicacion.getPais());
                            textLatitud.setText("Latitud: " + nuevaUbicacion.getLatitud());
                            textLongitud.setText("Longitud: " + nuevaUbicacion.getLongitud());
                            mostrarDatosUbicacion();
                            Toast.makeText(requireContext(), "Ubicación actualizada correctamente", Toast.LENGTH_SHORT).show();
                        }

                        @Override
                        public void onError(String mensaje) {
                            Toast.makeText(requireContext(), mensaje, Toast.LENGTH_SHORT).show();
                        }
                    });

                    Toast.makeText(requireContext(), "Ubicación registrada correctamente", Toast.LENGTH_SHORT).show();
                } else {
                    Toast.makeText(requireContext(), "Registro de ubicación cancelado", Toast.LENGTH_SHORT).show();
                }
            }
    );

    private void registrarUbicacion(Ubicacion nuevaUbicacion, CallbackGeneral callback) {
        String token = UsuarioSingleton.getInstancia().getToken();

        UbicacionServicios servicio = ClienteAPI.getRetrofit().create(UbicacionServicios.class);
        Call<ResponseBody> llamada = servicio.actualizarUbicacion(nuevaUbicacion, "Bearer " + token);

        HttpHelper.ejecutarHttp(llamada, new HttpCallback<ResponseBody>() {
            @Override
            public void onRespuesta(ResultadoHttp<ResponseBody> resultado) {
                if (resultado.exito) {
                    callback.onExito();
                } else {
                    callback.onError("Error al actualizar ubicación");
                }
            }
        });
    }

    private final ActivityResultLauncher<Intent> selectorFotoLauncher =
            registerForActivityResult(new ActivityResultContracts.StartActivityForResult(), result -> {
                if (result.getResultCode() == RESULT_OK && result.getData() != null) {
                    Uri uri = result.getData().getData();

                    ServicioMultimediaGrpcCliente cliente = new ServicioMultimediaGrpcCliente();
                    cliente.subirArchivoAsync(
                            requireContext(),
                            uri,
                            UsuarioSingleton.getInstancia().getUsuarioActual().getUsuarioID(),
                            new String[]{".jpg", ".jpeg", ".png"},
                            TIPO_SUBIDA_FOTO_USUARIO,
                            responseObserver -> cliente.getStub().subirFotoUsuario(responseObserver),
                            UsuarioSingleton.getInstancia().getToken(),
                            imageViewPreview
                    );
                }
            });

    public void seleccionarFoto() {
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        intent.setType("image/*");
        selectorFotoLauncher.launch(intent);
    }
}

