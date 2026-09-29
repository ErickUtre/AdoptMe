package com.adoptme.movil.dominio;

import com.adoptme.movil.dominio.Modelos.AdopcionResumen;

import java.time.OffsetDateTime;
import java.time.YearMonth;
import java.time.format.DateTimeParseException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.TreeMap;

public final class ConteoAdopciones {

    private ConteoAdopciones() {
    }

    public static List<ConteoMensual> porMes(List<AdopcionResumen> adopciones) {
        TreeMap<YearMonth, Integer> conteos = new TreeMap<>();
        for (AdopcionResumen adopcion : adopciones) {
            YearMonth mes = mesDe(adopcion.getFechaSolicitud());
            if (mes != null) {
                conteos.merge(mes, 1, Integer::sum);
            }
        }
        if (conteos.isEmpty()) {
            return Collections.emptyList();
        }
        List<ConteoMensual> resultado = new ArrayList<>();
        for (YearMonth mes = conteos.firstKey(); !mes.isAfter(conteos.lastKey()); mes = mes.plusMonths(1)) {
            resultado.add(new ConteoMensual(mes, conteos.getOrDefault(mes, 0)));
        }
        return resultado;
    }

    private static YearMonth mesDe(String fecha) {
        if (fecha == null) {
            return null;
        }
        try {
            return YearMonth.from(OffsetDateTime.parse(fecha));
        } catch (DateTimeParseException excepcion) {
            return null;
        }
    }

    public static final class ConteoMensual {
        private final YearMonth mes;
        private final int cantidad;

        public ConteoMensual(YearMonth mes, int cantidad) {
            this.mes = mes;
            this.cantidad = cantidad;
        }

        public YearMonth getMes() {
            return mes;
        }

        public int getCantidad() {
            return cantidad;
        }
    }
}
