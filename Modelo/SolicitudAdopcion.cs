using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class SolicitudAdopcion
    {
        [JsonProperty("SolicitudAdopcionID")]
        public int SolicitudAdopcionID {  get; set; }
        [JsonProperty("FechaSolicitud")]
        public DateTime FechaSolicitud {  get; set; }
        [JsonProperty("Estado")]
        public bool Estado {  get; set; }
        [JsonProperty("MascotaID")]
        public int MascotaID { get; set; }
        [JsonProperty("AdoptanteID")]
        public int AdoptanteID { get; set; }
    }
}
