package com.adoptme.movil.ui.reportes;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import com.adoptme.movil.R;
import com.adoptme.movil.databinding.FragmentReporteBinding;
import com.adoptme.movil.dominio.ConteoAdopciones.ConteoMensual;
import com.adoptme.movil.dominio.EstadoAdopcion;
import com.adoptme.movil.ui.comun.Pantallas;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.formatter.IndexAxisValueFormatter;

import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public final class ReporteFragment extends Fragment {

    private static final String ARGUMENTO_ESTADO = "estado";
    private static final String PATRON_MES = "MMM yyyy";

    private FragmentReporteBinding vista;

    static ReporteFragment nuevo(EstadoAdopcion estado) {
        Bundle argumentos = new Bundle();
        argumentos.putString(ARGUMENTO_ESTADO, estado.name());
        ReporteFragment fragmento = new ReporteFragment();
        fragmento.setArguments(argumentos);
        return fragmento;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflador, @Nullable ViewGroup contenedor, @Nullable Bundle estadoGuardado) {
        vista = FragmentReporteBinding.inflate(inflador, contenedor, false);
        return vista.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View raiz, @Nullable Bundle estadoGuardado) {
        EstadoAdopcion estado = EstadoAdopcion.valueOf(requireArguments().getString(ARGUMENTO_ESTADO));
        ReporteViewModel vistaModelo = Pantallas.vistaModelo(this, requireContext(), ReporteViewModel.class);
        Pantallas.observarEstado(getViewLifecycleOwner(), vistaModelo, vista.carga.getRoot(), requireContext());

        vista.textoTitulo.setText(estado == EstadoAdopcion.ADOPTADA ? R.string.reporte_adoptadas : R.string.reporte_en_adopcion);
        prepararGrafica();
        vistaModelo.getConteos().observe(getViewLifecycleOwner(), this::mostrar);
        vista.botonGuardar.setOnClickListener(boton ->
                vistaModelo.guardar(vista.grafica.getChartBitmap(), estado, System.currentTimeMillis()));
        if (estadoGuardado == null) {
            vistaModelo.cargar(estado);
        }
    }

    @Override
    public void onDestroyView() {
        vista = null;
        super.onDestroyView();
    }

    private void prepararGrafica() {
        vista.grafica.getDescription().setEnabled(false);
        vista.grafica.setNoDataText(getString(R.string.error_grafica_vacia));
        vista.grafica.setFitBars(true);
        vista.grafica.getAxisRight().setEnabled(false);
        vista.grafica.getAxisLeft().setAxisMinimum(0f);
        vista.grafica.getAxisLeft().setGranularity(1f);
        XAxis ejeX = vista.grafica.getXAxis();
        ejeX.setPosition(XAxis.XAxisPosition.BOTTOM);
        ejeX.setGranularity(1f);
        ejeX.setDrawGridLines(false);
    }

    private void mostrar(List<ConteoMensual> conteos) {
        if (conteos.isEmpty()) {
            vista.grafica.clear();
            return;
        }
        DateTimeFormatter formatoMes = DateTimeFormatter.ofPattern(PATRON_MES, Locale.getDefault());
        List<BarEntry> barras = new ArrayList<>();
        List<String> etiquetas = new ArrayList<>();
        for (int indice = 0; indice < conteos.size(); indice++) {
            ConteoMensual conteo = conteos.get(indice);
            barras.add(new BarEntry(indice, conteo.getCantidad()));
            etiquetas.add(formatoMes.format(conteo.getMes()));
        }
        BarDataSet serie = new BarDataSet(barras, getString(R.string.leyenda_grafica));
        serie.setColor(ContextCompat.getColor(requireContext(), R.color.reporte));
        serie.setValueTextSize(12f);
        vista.grafica.getXAxis().setValueFormatter(new IndexAxisValueFormatter(etiquetas));
        vista.grafica.getXAxis().setLabelCount(etiquetas.size());
        vista.grafica.setData(new BarData(serie));
        vista.grafica.invalidate();
    }
}
