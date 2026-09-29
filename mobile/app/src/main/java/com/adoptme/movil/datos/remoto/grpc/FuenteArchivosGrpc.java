package com.adoptme.movil.datos.remoto.grpc;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.provider.OpenableColumns;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.comun.Resultado;
import com.adoptme.movil.comun.TipoError;
import com.adoptme.movil.datos.repositorios.ArchivoRepositorio;
import com.adoptme.movil.grpc.multimedia.FragmentoArchivo;
import com.adoptme.movil.grpc.multimedia.MetadatosArchivo;
import com.adoptme.movil.grpc.multimedia.ResultadoSubida;
import com.adoptme.movil.grpc.multimedia.ServicioMultimediaGrpc;
import com.google.protobuf.ByteString;

import java.io.IOException;
import java.io.InputStream;
import java.util.concurrent.Executor;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.function.Function;

import io.grpc.stub.StreamObserver;

public final class FuenteArchivosGrpc implements ArchivoRepositorio {

    private static final int TAMANO_FRAGMENTO = 64 * 1024;

    private final CanalGrpc canal;
    private final ContentResolver resolutor;
    private final Executor ejecutor;

    public FuenteArchivosGrpc(CanalGrpc canal, ContentResolver resolutor, Executor ejecutor) {
        this.canal = canal;
        this.resolutor = resolutor;
        this.ejecutor = ejecutor;
    }

    @Override
    public void subirFotoPerfil(Uri archivo, AlTerminar<Void> alTerminar) {
        subir(stub()::subirFotoUsuario, 0, archivo, alTerminar);
    }

    @Override
    public void subirFotoMascota(int mascotaId, Uri archivo, AlTerminar<Void> alTerminar) {
        subir(stub()::subirFotoMascota, mascotaId, archivo, alTerminar);
    }

    @Override
    public void subirVideoMascota(int mascotaId, Uri archivo, AlTerminar<Void> alTerminar) {
        subir(stub()::subirVideoMascota, mascotaId, archivo, alTerminar);
    }

    private ServicioMultimediaGrpc.ServicioMultimediaStub stub() {
        return ServicioMultimediaGrpc.newStub(canal.getCanal());
    }

    private void subir(
            Function<StreamObserver<ResultadoSubida>, StreamObserver<FragmentoArchivo>> abrirFlujo,
            int idReferencia,
            Uri archivo,
            AlTerminar<Void> alTerminar) {
        ejecutor.execute(() -> {
            ObservadorRespuesta respuesta = new ObservadorRespuesta(alTerminar);
            StreamObserver<FragmentoArchivo> envio = abrirFlujo.apply(respuesta);
            try (InputStream entrada = resolutor.openInputStream(archivo)) {
                if (entrada == null) {
                    throw new IOException("No se pudo abrir el archivo seleccionado");
                }
                envio.onNext(FragmentoArchivo.newBuilder()
                        .setMetadatos(MetadatosArchivo.newBuilder().setIdReferencia(idReferencia).setNombreArchivo(nombreDe(archivo)))
                        .build());
                byte[] bufer = new byte[TAMANO_FRAGMENTO];
                int leidos;
                while ((leidos = entrada.read(bufer)) != -1) {
                    envio.onNext(FragmentoArchivo.newBuilder().setDatos(ByteString.copyFrom(bufer, 0, leidos)).build());
                }
                envio.onCompleted();
            } catch (IOException excepcion) {
                respuesta.completar(Resultado.fallo(TipoError.VALIDACION, excepcion.getMessage()));
                envio.onError(excepcion);
            }
        });
    }

    private String nombreDe(Uri archivo) {
        try (Cursor cursor = resolutor.query(archivo, new String[]{OpenableColumns.DISPLAY_NAME}, null, null, null)) {
            if (cursor != null && cursor.moveToFirst()) {
                return cursor.getString(0);
            }
        }
        String segmento = archivo.getLastPathSegment();
        return segmento == null ? "archivo" : segmento;
    }

    private static final class ObservadorRespuesta implements StreamObserver<ResultadoSubida> {

        private final AlTerminar<Void> alTerminar;
        private final AtomicBoolean completado = new AtomicBoolean(false);
        private Resultado<Void> resultado = Resultado.fallo(TipoError.DESCONOCIDO, null);

        ObservadorRespuesta(AlTerminar<Void> alTerminar) {
            this.alTerminar = alTerminar;
        }

        void completar(Resultado<Void> definitivo) {
            if (completado.compareAndSet(false, true)) {
                alTerminar.con(definitivo);
            }
        }

        @Override
        public void onNext(ResultadoSubida respuesta) {
            resultado = respuesta.getExito()
                    ? Resultado.exito(null)
                    : Resultado.fallo(TipoError.DESCONOCIDO, respuesta.getMensaje());
        }

        @Override
        public void onError(Throwable causa) {
            completar(Resultado.fallo(CanalGrpc.traducir(causa)));
        }

        @Override
        public void onCompleted() {
            completar(resultado);
        }
    }
}
