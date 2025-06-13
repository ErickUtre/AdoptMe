using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Mensaje
    {
        [JsonProperty("MensajeID")]
        public int MensajeID { get; set; }
        [JsonProperty("RemitenteID")]
        public int RemitenteID { get; set; }
        [JsonProperty("ReceptorID")]
        public int ReceptorID { get; set; }
        [JsonProperty("FechaEnvio")]
        public DateTime FechaEnvio { get; set; }
        [JsonProperty("Contenido")]
        public string Contenido { get; set; }
        [JsonProperty("Leido")]
        public bool Leido { get; set; }
    }
}
