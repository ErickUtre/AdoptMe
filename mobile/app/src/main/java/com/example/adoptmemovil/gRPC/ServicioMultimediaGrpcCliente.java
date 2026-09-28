package com.example.adoptmemovil.gRPC;

import static com.example.adoptmemovil.utilidades.Constantes.DIRECCION_IP;
import static com.example.adoptmemovil.utilidades.Constantes.TIPO_SUBIDA_FOTO_USUARIO;
import static com.example.adoptmemovil.utilidades.InterfazUsuarioUtils.cargarFotoPerfil;

import android.app.AlertDialog;
import android.app.Activity;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.os.Handler;
import android.os.Looper;
import android.provider.OpenableColumns;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import com.example.adoptmemovil.R;
import com.example.adoptmemovil.utilidades.HeaderClientInterceptor;
import com.example.adoptmemovil.utilidades.MetodoSubida;
import com.example.adoptmemovil.utilidades.UsuarioSingleton;
import com.google.protobuf.ByteString;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.util.Arrays;
import java.util.concurrent.Executors;
import java.util.function.Function;

import io.grpc.Channel;
import io.grpc.ClientInterceptors;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.Metadata;
import io.grpc.stub.MetadataUtils;
import io.grpc.stub.StreamObserver;
import multimedia.ChunkArchivo;
import multimedia.RespuestaGeneral;
import multimedia.ServicioMultimediaGrpc;

public class ServicioMultimediaGrpcCliente {
    private final ServicioMultimediaGrpc.ServicioMultimediaStub stub;
    public ServicioMultimediaGrpcCliente() {
        ManagedChannel canal = ManagedChannelBuilder
                .forAddress(DIRECCION_IP, 50051)
                .usePlaintext()
                .build();

        this.stub = ServicioMultimediaGrpc
                .newStub(ClientInterceptors.intercept(canal, new HeaderClientInterceptor(UsuarioSingleton.getInstancia().getToken())));
    }

    public ServicioMultimediaGrpc.ServicioMultimediaStub getStub() {
        return this.stub;
    }

    public void subirArchivoAsync(
            Context context,
            Uri uri,
            int idReferencia,
            String[] extensionesPermitidas,
            String tipoSubida,
            MetodoSubida metodoSubida,
            String token,
            ImageView imageView
    ) {
        Executors.newSingleThreadExecutor().execute(() -> {
            try {
                String nombreArchivo = obtenerNombreArchivoDesdeUri(context, uri);
                if (nombreArchivo == null) {
                    mostrarToast(context, "No se pudo obtener el nombre del archivo");
                    return;
                }

                String extension = nombreArchivo.substring(nombreArchivo.lastIndexOf(".")).toLowerCase();
                if (!Arrays.asList(extensionesPermitidas).contains(extension)) {
                    mostrarToast(context, "Formato no permitido: " + extension);
                    return;
                }

                StreamObserver<RespuestaGeneral> responseObserver =
                        new StreamObserver<RespuestaGeneral>() {
                            @Override
                            public void onNext(RespuestaGeneral respuesta) {
                                Log.d("gRPC", respuesta.getMensaje());

                            }

                            @Override
                            public void onError(Throwable t) {
                                String mensajeError;
                                if (t instanceof io.grpc.StatusRuntimeException) {
                                    io.grpc.StatusRuntimeException sre = (io.grpc.StatusRuntimeException) t;
                                    mensajeError = "Código: " + sre.getStatus().getCode() + "\nDescripción: " + sre.getStatus().getDescription();
                                } else {
                                    mensajeError = t.toString();
                                }
                                mostrarToast(context, "Error al subir archivo");
                            }

                            @Override
                            public void onCompleted() {
                                UsuarioSingleton.getInstancia().getUsuarioActual().limpiarFoto();
                                cargarFotoPerfil(context, token, imageView);

                                if (context instanceof Activity) {
                                    Activity actividad = (Activity) context;
                                    ImageView iconoMenu = actividad.findViewById(R.id.iv_perfil);
                                    if (iconoMenu != null) {
                                        cargarFotoPerfil(actividad, token, iconoMenu);
                                    }
                                }

                                Log.d("gRPC", "Transferencia completada");
                            }
                        };

                StreamObserver<ChunkArchivo> requestObserver = metodoSubida.aplicar(responseObserver);

                multimedia.ChunkArchivo metadataChunk = multimedia.ChunkArchivo.newBuilder()
                        .setMetadata(multimedia.Metadata.newBuilder()
                                .setIdReferencia(idReferencia)
                                .setNombreArchivo(nombreArchivo)
                                .build())
                        .build();

                requestObserver.onNext(metadataChunk);

                InputStream inputStream = context.getContentResolver().openInputStream(uri);
                byte[] buffer = new byte[64 * 1024];
                int bytesRead;
                while ((bytesRead = inputStream.read(buffer)) != -1) {
                    try {
                        requestObserver.onNext(ChunkArchivo.newBuilder()
                                .setChunk(ByteString.copyFrom(buffer, 0, bytesRead))
                                .build());
                    } catch (Exception ex) {
                        Log.e("gRPC", "Error al enviar chunk", ex);
                        break;
                    }
                }

                inputStream.close();
                requestObserver.onCompleted();

            } catch (Exception e) {
                mostrarToast(context, "Error al subir archivo: " + e.getMessage());
            }
        });
    }

    public String obtenerNombreArchivoDesdeUri(Context context, Uri uri) {
        String resultado = null;
        if (uri.getScheme().equals("content")) {
            try (Cursor cursor = context.getContentResolver().query(uri, null, null, null, null)) {
                if (cursor != null && cursor.moveToFirst()) {
                    int index = cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME);
                    if (index != -1) {
                        resultado = cursor.getString(index);
                    }
                }
            }
        }
        if (resultado == null) {
            resultado = uri.getLastPathSegment();
        }
        return resultado;
    }

    private void mostrarToast(Context context, String mensaje) {
        new Handler(Looper.getMainLooper()).post(() ->
                Toast.makeText(context, mensaje, Toast.LENGTH_LONG).show());
    }
}
