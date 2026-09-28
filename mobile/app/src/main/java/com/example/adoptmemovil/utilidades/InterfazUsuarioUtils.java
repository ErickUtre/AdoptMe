package com.example.adoptmemovil.utilidades;

import static com.example.adoptmemovil.utilidades.Constantes.DIRECCION_IP;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.annotation.OptIn;
import androidx.media3.common.MediaItem;
import androidx.media3.common.util.UnstableApi;
import androidx.media3.datasource.DefaultHttpDataSource;
import androidx.media3.exoplayer.ExoPlayer;
import androidx.media3.exoplayer.source.DefaultMediaSourceFactory;
import androidx.media3.ui.PlayerView;

import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.MascotaServicios;
import com.example.adoptmemovil.servicios.UsuarioServicios;

import java.util.HashMap;
import java.util.Map;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class InterfazUsuarioUtils {

    private static final String BASE_URL = "http://" + DIRECCION_IP + ":8080/api";

    public static Drawable escalarDrawable(Context context, Drawable drawable, float escala) {
        if (drawable == null) return null;

        Bitmap originalBitmap = Bitmap.createBitmap(
                drawable.getIntrinsicWidth(),
                drawable.getIntrinsicHeight(),
                Bitmap.Config.ARGB_8888
        );

        Canvas canvas = new Canvas(originalBitmap);
        drawable.setBounds(0, 0, canvas.getWidth(), canvas.getHeight());
        drawable.draw(canvas);

        int nuevoAncho = (int) (canvas.getWidth() * escala);
        int nuevoAlto = (int) (canvas.getHeight() * escala);

        Bitmap escalado = Bitmap.createScaledBitmap(originalBitmap, nuevoAncho, nuevoAlto, true);
        return new BitmapDrawable(context.getResources(), escalado);
    }

    public static void cargarFotoMascota(Context context, String token, int mascotaId, ImageView destino) {
        if (FotoMascotaCache.contieneFoto(mascotaId)) {
            destino.setImageBitmap(FotoMascotaCache.obtenerFoto(mascotaId));
            return;
        }

        MascotaServicios servicio = ClienteAPI.getRetrofit().create(MascotaServicios.class);
        Call<ResponseBody> llamada = servicio.obtenerFotoMascota("Bearer " + token, mascotaId);

        llamada.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    new Thread(() -> {
                        try {
                            Bitmap bitmap = BitmapFactory.decodeStream(response.body().byteStream());

                            new Handler(Looper.getMainLooper()).post(() -> {
                                destino.setImageBitmap(bitmap);
                                FotoMascotaCache.guardarFoto(mascotaId, bitmap);
                            });
                        } catch (Exception e) {
                            Log.e("FotoMascotaHelper", "Error al decodificar la imagen", e);
                        }
                    }).start();
                } else {
                    Log.e("FotoMascotaHelper", "Error HTTP: " + response.code());
                    Toast.makeText(context, "No se pudo obtener la foto", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Log.e("FotoMascotaHelper", "Fallo al obtener la foto", t);
                Toast.makeText(context, "Error de conexión al obtener la foto", Toast.LENGTH_SHORT).show();
            }
        });
    }

    public static void cargarFotoPerfil(Context context, String token, ImageView destino) {
        if (UsuarioSingleton.getInstancia().getUsuarioActual().hayFoto()) {
            destino.setImageBitmap(UsuarioSingleton.getInstancia().getUsuarioActual().getFotoPerfil());
            return;
        }

        UsuarioServicios servicio = ClienteAPI.getRetrofit().create(UsuarioServicios.class);
        Call<ResponseBody> llamada = servicio.obtenerFotoPerfil("Bearer " + token);

        llamada.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    new Thread(() -> {
                        try {
                            Bitmap bitmap = BitmapFactory.decodeStream(response.body().byteStream());

                            new Handler(Looper.getMainLooper()).post(() -> {
                                destino.setImageBitmap(bitmap);
                                UsuarioSingleton.getInstancia().getUsuarioActual().setFotoPerfil(bitmap);
                            });
                        } catch (Exception e) {
                            Log.e("FotoPerfilHelper", "Error al decodificar la imagen", e);
                        }
                    }).start();
                } else {
                    Log.e("FotoPerfilHelper", "Error HTTP: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Log.e("FotoPerfilHelper", "Fallo al obtener la foto", t);
                Toast.makeText(context, "Error de conexión al obtener la foto", Toast.LENGTH_SHORT).show();
            }
        });
    }

    @OptIn(markerClass = UnstableApi.class)
    public static void reproducirVideoMascota(Context context, String token, int mascotaId, PlayerView playerView) {
        String videoUrl = BASE_URL + "/mascotas/" + mascotaId + "/video";

        Map<String, String> headers = new HashMap<>();
        headers.put("Authorization", "Bearer " + token);

        DefaultHttpDataSource.Factory dataSourceFactory = new DefaultHttpDataSource.Factory()
                .setDefaultRequestProperties(headers);

        ExoPlayer player = new ExoPlayer.Builder(context)
                .setMediaSourceFactory(new DefaultMediaSourceFactory(dataSourceFactory))
                .build();

        MediaItem mediaItem = MediaItem.fromUri(videoUrl);
        player.setMediaItem(mediaItem);
        player.prepare();
        player.play();

        playerView.setPlayer(player);
    }
}
