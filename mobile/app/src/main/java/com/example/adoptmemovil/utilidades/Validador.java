package com.example.adoptmemovil.utilidades;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Validador {
    private static final Pattern nombrePattern = Pattern.compile(
            "^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$");

    private static final Pattern correoPattern = Pattern.compile(
            "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$");

    private static final Pattern telefonoPattern = Pattern.compile(
            "^[0-9]{10}$");

    public static boolean validarNombre(String nombre) {
        if (nombre == null) return false;
        String limpio = nombre.trim().replaceAll("\\s+", " ");
        return validarRegex(limpio, nombrePattern);
    }

    public static boolean validarCorreo(String correo) {
        if (correo == null) return false;
        String limpio = correo.trim().replaceAll("\\s+", " ");
        return validarRegex(limpio, correoPattern);
    }

    public static boolean validarTelefono(String telefono) {
        if (telefono == null) return false;
        String limpio = telefono.trim().replaceAll("\\s+", " ");
        return validarRegex(limpio, telefonoPattern);
    }

    public static boolean validarContrasena(String contrasena) {
        if (contrasena == null || contrasena.contains(" ")) return false;
        if (contrasena.length() < 8) return false;

        boolean tieneMayuscula = false;
        int digitos = 0;

        for (char c : contrasena.toCharArray()) {
            if (Character.isUpperCase(c)) tieneMayuscula = true;
            if (Character.isDigit(c)) digitos++;
        }

        return tieneMayuscula && digitos >= 2;
    }

    public static boolean esMismaContrasena(String c1, String c2) {
        return c1 != null && c1.equals(c2);
    }

    private static boolean validarRegex(String texto, Pattern pattern) {
        try {
            Matcher matcher = pattern.matcher(texto);
            return matcher.matches();
        } catch (Exception e) {
            return false;
        }
    }
}
