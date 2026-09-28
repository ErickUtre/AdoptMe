using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Notificacion
    {
        [JsonProperty("NotificacionId")]
        public int NotificacionId { get; set; }
        [JsonProperty("Titulo")]
        public string Titulo {  get; set; }
        [JsonProperty("Mensaje")]
        public string Mensaje { get; set; }
        [JsonProperty("Tipo")]
        public string Tipo { get; set; }
        [JsonProperty("ReferenciaId")]
        public int? ReferenciaId { get; set; }
        [JsonProperty("ReferenciaTipo")]
        public string ReferenciaTipo { get; set; }
        [JsonProperty("FechaCreacion")]
        public DateTime Fecha { get; set; }
    }
}
