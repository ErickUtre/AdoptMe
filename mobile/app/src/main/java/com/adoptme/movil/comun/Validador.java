package com.adoptme.movil.comun;

import java.util.regex.Pattern;

public final class Validador {

    private static final Pattern NOMBRE = Pattern.compile("^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ'’-]+(?:\\s[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ'’-]+)*$");
    private static final Pattern CORREO = Pattern.compile("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$");
    private static final Pattern TELEFONO = Pattern.compile("^\\d{10}$");
    private static final int LONGITUD_MINIMA_CONTRASENA = 8;
    private static final int DIGITOS_MINIMOS_CONTRASENA = 2;

    private Validador() {
    }

    public static String normalizar(String texto) {
        return texto == null ? "" : texto.trim().replaceAll("\\s+", " ");
    }

    public static boolean esNombreValido(String nombre) {
        return NOMBRE.matcher(normalizar(nombre)).matches();
    }

    public static boolean esCorreoValido(String correo) {
        return correo != null && CORREO.matcher(correo.trim()).matches();
    }

    public static boolean esTelefonoValido(String telefono) {
        return telefono != null && TELEFONO.matcher(telefono.trim()).matches();
    }

    public static boolean esContrasenaValida(String contrasena) {
        if (contrasena == null || contrasena.length() < LONGITUD_MINIMA_CONTRASENA) {
            return false;
        }
        boolean tieneMayuscula = false;
        int digitos = 0;
        for (char caracter : contrasena.toCharArray()) {
            if (Character.isWhitespace(caracter)) {
                return false;
            }
            tieneMayuscula |= Character.isUpperCase(caracter);
            digitos += Character.isDigit(caracter) ? 1 : 0;
        }
        return tieneMayuscula && digitos >= DIGITOS_MINIMOS_CONTRASENA;
    }
}
