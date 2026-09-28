using System;
using Cliente_AdoptMe.Modelo; // Asegúrate de tener el namespace correcto para Mensaje

namespace Cliente_AdoptMe.Modelo
{
    public class MensajeUI
    {
        public string Contenido { get; set; }
        public string Hora { get; set; }
        public bool EsPropio { get; set; }
        public int RemitenteID { get; set; }
        public DateTime FechaEnvio { get; set; }

        public MensajeUI() { }

        public MensajeUI(Mensaje mensaje, int usuarioActualID)
        {
            Contenido = mensaje.Contenido;
            FechaEnvio = mensaje.FechaEnvio;
            Hora = mensaje.FechaEnvio.ToString("HH:mm");
            EsPropio = mensaje.RemitenteID == usuarioActualID;
            RemitenteID = mensaje.RemitenteID;
        }
    }
}
