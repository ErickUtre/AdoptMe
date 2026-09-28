package com.example.adoptmemovil;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.example.adoptmemovil.modelo.SolicitudAdopcion;
import com.example.adoptmemovil.servicios.ClienteAPI;
import com.example.adoptmemovil.servicios.SolicitudAdopcionServicios;
import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.formatter.IndexAxisValueFormatter;

import java.io.IOException;
import java.io.OutputStream;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ReporteFragment extends Fragment {

    private BarChart barChart;
    private Button btnGuardar;
    private boolean mostrarAdoptadas = true;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_reporte, container, false);

        barChart = view.findViewById(R.id.barChart);
        btnGuardar = view.findViewById(R.id.btnGuardar);

        if (getArguments() != null) {
            mostrarAdoptadas = getArguments().getBoolean("estado", true);
        }

        cargarDatosDesdeServidor();

        btnGuardar.setOnClickListener(v -> guardarGrafica());

        return view;
    }

    private void cargarDatosDesdeServidor() {
        SolicitudAdopcionServicios service = ClienteAPI.getRetrofit().create(SolicitudAdopcionServicios.class);
        Call<List<SolicitudAdopcion>> call = mostrarAdoptadas ?
                service.obtenerSolicitudesAceptadas() :
                service.obtenerSolicitudesPendientes();

        call.enqueue(new Callback<List<SolicitudAdopcion>>() {
            @Override
            public void onResponse(Call<List<SolicitudAdopcion>> call, Response<List<SolicitudAdopcion>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    procesarDatos(response.body());
                } else {
                    Toast.makeText(getContext(), "Error al cargar datos", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<List<SolicitudAdopcion>> call, Throwable t) {
                Toast.makeText(getContext(), "Error de conexión", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void procesarDatos(List<SolicitudAdopcion> lista) {
        int[] conteoPorMes = new int[12];
        SimpleDateFormat formato = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.US);

        for (SolicitudAdopcion solicitud : lista) {
            String fechaStr = solicitud.getFecha();
            try {
                Date fecha = formato.parse(fechaStr);
                Calendar calendar = Calendar.getInstance();
                calendar.setTime(fecha);
                int mes = calendar.get(Calendar.MONTH);
                conteoPorMes[mes]++;
            } catch (ParseException e) {
                e.printStackTrace();
            }
        }

        List<BarEntry> entries = new ArrayList<>();
        for (int i = 0; i < 12; i++) {
            entries.add(new BarEntry(i, conteoPorMes[i]));
        }

        BarDataSet dataSet = new BarDataSet(entries, "Mascotas adoptadas");
        dataSet.setColor(Color.BLUE);
        BarData data = new BarData(dataSet);
        barChart.setData(data);

        String[] meses = new String[]{"Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"};
        XAxis xAxis = barChart.getXAxis();
        xAxis.setValueFormatter(new IndexAxisValueFormatter(meses));
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setGranularity(1f);
        xAxis.setGranularityEnabled(true);

        barChart.getDescription().setEnabled(false);
        barChart.invalidate();
    }

    private void guardarGrafica() {
        Bitmap bitmap = barChart.getChartBitmap();
        if (bitmap == null) {
            Toast.makeText(requireContext(), "Error: El gráfico está vacío", Toast.LENGTH_LONG).show();
            return;
        }

        // Generar nombre único para el archivo
        String nombreArchivo = "grafica_adopciones_" + System.currentTimeMillis() + ".png";

        // Crear los valores para insertar en MediaStore
        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, nombreArchivo);
        values.put(MediaStore.Images.Media.MIME_TYPE, "image/png");
        values.put(MediaStore.Images.Media.RELATIVE_PATH, Environment.DIRECTORY_PICTURES + "/AdoptMe");

        // Obtener el content resolver e insertar el archivo
        ContentResolver resolver = requireContext().getContentResolver();
        Uri uri = resolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);

        if (uri == null) {
            Toast.makeText(requireContext(), "Error al crear archivo de imagen", Toast.LENGTH_LONG).show();
            return;
        }

        try (OutputStream out = resolver.openOutputStream(uri)) {
            if (out == null) {
                Toast.makeText(requireContext(), "Error al abrir flujo de salida", Toast.LENGTH_LONG).show();
                return;
            }

            boolean success = bitmap.compress(Bitmap.CompressFormat.PNG, 100, out);
            if (success) {
                Toast.makeText(requireContext(), "Imagen guardada en Galería", Toast.LENGTH_SHORT).show();
            } else {
                Toast.makeText(requireContext(), "Error al comprimir imagen", Toast.LENGTH_LONG).show();
            }
        } catch (IOException e) {
            Toast.makeText(requireContext(), "IOException: " + e.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

}
