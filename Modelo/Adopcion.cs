using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Adopcion
    {
        [JsonProperty("AdopcionID")]
        public int AdopcionID {  get; set; }
        [JsonProperty("FechaSolicitud")]
        public DateTime FechaSolicitud {  get; set; }
        [JsonProperty("Estado")]
        public bool Estado {  get; set; }
        [JsonProperty("MascotaID")]
        public int MascotaID { get; set; }
        [JsonProperty("PublicadorID")]
        public int PublicadorID { get; set; }
        [JsonProperty("Ubicacion")]
        public Ubicacion Ubicacion { get; set; }
        [JsonProperty("Mascota")]
        public Mascota Mascota { get; set; }
    }

    public class AdopcionCercana
    {
        [JsonProperty("adopcionId")]
        public string AdopcionId { get; set; }
        [JsonProperty("distancia")]
        public double? Distancia { get; set; }
        [JsonProperty("latitud")]
        public double? Latitud { get; set; }
        [JsonProperty("longitud")]
        public double? Longitud { get; set; }
    }
}
