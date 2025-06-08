using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Acceso
    {
        [JsonProperty("AccesoID")]
        public int AccesoID { get; set; }
        [JsonProperty("Correo")]
        public string Correo { get; set; }
        [JsonProperty("ContrasenaHash")]
        public string ContrasenaHash { get; set; }
        [JsonProperty("EsAdmin")]
        public bool EsAdmin { get; set; }
    }
}
