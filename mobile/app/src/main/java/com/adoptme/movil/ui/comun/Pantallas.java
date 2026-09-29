package com.adoptme.movil.ui.comun;

import android.content.Context;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import androidx.activity.ComponentActivity;
import androidx.activity.EdgeToEdge;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.lifecycle.LifecycleOwner;
import androidx.lifecycle.ViewModel;
import androidx.lifecycle.ViewModelProvider;
import androidx.lifecycle.ViewModelStoreOwner;

import com.adoptme.movil.AdoptMeAplicacion;
import com.adoptme.movil.R;
import com.adoptme.movil.dominio.Ubicacion;

public final class Pantallas {

    private static final String SIN_DATO = "—";

    private Pantallas() {
    }

    public static void mostrarContenido(ComponentActivity actividad, View raiz) {
        EdgeToEdge.enable(actividad);
        actividad.setContentView(raiz);
        int izquierda = raiz.getPaddingLeft();
        int arriba = raiz.getPaddingTop();
        int derecha = raiz.getPaddingRight();
        int abajo = raiz.getPaddingBottom();
        ViewCompat.setOnApplyWindowInsetsListener(raiz, (vista, margenes) -> {
            Insets sistema = margenes.getInsets(WindowInsetsCompat.Type.systemBars()
                    | WindowInsetsCompat.Type.displayCutout()
                    | WindowInsetsCompat.Type.ime());
            vista.setPadding(izquierda + sistema.left, arriba + sistema.top, derecha + sistema.right, abajo + sistema.bottom);
            return WindowInsetsCompat.CONSUMED;
        });
    }

    public static <T extends ViewModel> T vistaModelo(ViewModelStoreOwner propietario, Context contexto, Class<T> tipo) {
        return new ViewModelProvider(propietario, AdoptMeAplicacion.contenedor(contexto).getFabricaViewModels()).get(tipo);
    }

    public static void observarEstado(LifecycleOwner propietario, VistaModeloBase vistaModelo, View capaCarga, Context contexto) {
        vistaModelo.getCargando().observe(propietario, cargando -> capaCarga.setVisibility(Boolean.TRUE.equals(cargando) ? View.VISIBLE : View.GONE));
        vistaModelo.getAvisos().observe(propietario, evento -> {
            Aviso aviso = evento.consumir();
            if (aviso != null) {
                Toast.makeText(contexto, aviso.describir(contexto), Toast.LENGTH_LONG).show();
            }
        });
    }

    public static String texto(EditText campo) {
        return campo.getText() == null ? "" : campo.getText().toString().trim();
    }

    public static String contrasena(EditText campo) {
        return campo.getText() == null ? "" : campo.getText().toString();
    }

    public static String describir(Context contexto, Ubicacion ubicacion) {
        return contexto.getString(R.string.descripcion_ubicacion,
                valor(ubicacion.getCiudad()), valor(ubicacion.getEstado()), valor(ubicacion.getPais()),
                ubicacion.getLatitud(), ubicacion.getLongitud());
    }

    public static String valor(String texto) {
        return texto == null || texto.isEmpty() ? SIN_DATO : texto;
    }
}
