package com.adoptme.movil.comun;

public enum TipoError {
    VALIDACION,
    NO_AUTENTICADO,
    PROHIBIDO,
    NO_ENCONTRADO,
    CONFLICTO,
    SERVICIO_NO_DISPONIBLE,
    CONEXION,
    DESCONOCIDO;

    public static TipoError desdeCodigoHttp(int codigo) {
        switch (codigo) {
            case 400:
                return VALIDACION;
            case 401:
                return NO_AUTENTICADO;
            case 403:
                return PROHIBIDO;
            case 404:
                return NO_ENCONTRADO;
            case 409:
                return CONFLICTO;
            case 503:
                return SERVICIO_NO_DISPONIBLE;
            default:
                return DESCONOCIDO;
        }
    }
}
