namespace AdoptMe.Escritorio.Nucleo;

public static class Textos
{
    public const string Aplicacion = "AdoptMe";
    public const string Exito = "Éxito";
    public const string Error = "Error";
    public const string Advertencia = "Advertencia";
    public const string Confirmacion = "Confirmación";

    public const string ErrorConexion = "No fue posible conectar con el servidor. Intenta más tarde.";
    public const string ErrorTiempoAgotado = "La solicitud tardó demasiado en responder.";
    public const string ErrorRespuestaInvalida = "El servidor devolvió una respuesta inesperada.";
    public const string ErrorServidor = "Ocurrió un error en el servidor.";
    public const string ErrorInesperado = "Ocurrió un error inesperado y la aplicación se cerrará. El detalle quedó registrado.";
    public const string ErrorUbicacion = "Error al obtener la ubicación, intenta más tarde.";

    public const string CredencialesIncorrectas = "El correo y/o la contraseña son incorrectos, favor de verificar.";
    public const string CamposObligatorios = "Por favor, completa correctamente los campos marcados.";
    public const string RegistroExitoso = "Registro exitoso.";
    public const string CorreoRegistrado = "El correo ya está registrado.";
    public const string PerfilActualizado = "Perfil actualizado correctamente.";
    public const string ReglasContrasena = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, dos números y ningún espacio.";

    public const string PermitirUbicacion = "¿Deseas permitir que obtengamos tu ubicación?";
    public const string ConfirmarModificarUbicacion = "¿Seguro que deseas modificar la ubicación?";
    public const string UbicacionImprecisa = "La ubicación puede no ser precisa. Da doble clic en el mapa para seleccionar tu ubicación.";
    public const string SoloMexico = "Por ahora solo hay soporte para ubicaciones dentro de México.";
    public const string SeleccionaUbicacion = "Selecciona una ubicación antes de continuar.";

    public const string SinAdopcionesRegistradas = "Aún no has registrado adopciones.";
    public const string AdopcionRegistrada = "Adopción registrada correctamente.";
    public const string AdopcionEliminada = "Adopción eliminada correctamente.";
    public const string CampoActualizado = "Campo actualizado correctamente.";
    public const string SolicitudEnviada = "Solicitud enviada correctamente.";
    public const string SolicitudDuplicada = "Ya has enviado una solicitud para esta adopción.";
    public const string SinSolicitudes = "No hay solicitudes pendientes para esta adopción.";
    public const string SolicitudAceptada = "Solicitud aceptada y adopción actualizada.";
    public const string SolicitudRechazada = "Solicitud rechazada.";

    public const string SinVideo = "No hay video disponible para esta mascota.";
    public const string FotoObligatoria = "Selecciona una foto de la mascota.";
    public const string ReporteGenerado = "Reporte generado correctamente.";
    public const string SinDatosReporte = "No hay datos para generar el reporte.";

    public const string Disponible = "Disponible";
    public const string Adoptado = "Adoptado";
    public const string UsuarioDesconocido = "Usuario";
    public const string MostrarUbicacionActual = "Mostrar ubicación actual";
    public const string MostrarUbicacionRegistrada = "Mostrar ubicación registrada";
    public const string ReporteAdoptadas = "Reporte de mascotas adoptadas";
    public const string ReporteEnAdopcion = "Reporte de mascotas en adopción";
    public const string Mes = "Mes";
    public const string Cantidad = "Cantidad";

    public const string CampoVacio = "El campo no puede estar vacío.";
    public const string SeleccionaOpcion = "Selecciona una opción válida.";
    public const string NombreInvalido = "Ingresa un nombre válido.";
    public const string CorreoInvalido = "Ingresa un correo válido.";
    public const string TelefonoInvalido = "El teléfono debe tener 10 dígitos.";

    public static string ConfirmarEliminarAdopcion(string nombreMascota) => $"¿Seguro que deseas eliminar la adopción de '{nombreMascota}'?";

    public static string ConfirmarRechazo(string nombreAdoptante) => $"¿Deseas rechazar la solicitud de {nombreAdoptante}?";

    public static string ConfirmarAceptacion(string nombreAdoptante) => $"¿Aceptar la solicitud de {nombreAdoptante} y marcar la mascota como adoptada?";

    public static string ArchivoNoSubido(string motivo) => $"La adopción se registró, pero no se pudo subir el archivo: {motivo}";
}
