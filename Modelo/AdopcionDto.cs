using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class AdopcionDto
    {
        [JsonProperty("FechaSolicitud")]
        public DateTime? FechaSolicitud { get; set; }
        [JsonProperty("Estado")]
        public bool Estado { get; set; }
    }
}
