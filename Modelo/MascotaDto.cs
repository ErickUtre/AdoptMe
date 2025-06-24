using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class MascotaDto
    {
        [JsonProperty("MascotaID")]
        public int MascotaID { get; set; }
    }
}
