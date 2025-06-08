using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Usuario
    {
        [JsonProperty("UsuarioID")]
        public int UsuarioId { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }
        [JsonProperty("Telefono")]
        public string Telefono { get; set; }
        [JsonProperty("Ubicacion")]
        public Ubicacion Ubicacion { get; set; }
        [JsonProperty("Acceso")]
        public Acceso Acceso { get; set; }
    }
}
