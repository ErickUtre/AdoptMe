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
        [JsonProperty("ChatID")]
        public int ChatID { get; set; }

        [JsonProperty("RemitenteID")]
        public int RemitenteID { get; set; }

        [JsonProperty("DestinatarioID")]
        public int DestinatarioID { get; set; }

        [JsonProperty("Contenido")]
        public string Contenido { get; set; }

        [JsonProperty("FechaEnvio")]
        public DateTime FechaEnvio { get; set; }
    }

}
