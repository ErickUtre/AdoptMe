using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Mascota
    {
        [JsonProperty("MascotaID")]
        public int MascotaID { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("Especie")]
        public string Especie { get; set; }
        [JsonProperty("Raza")]
        public string Raza { get; set; }
        [JsonProperty("Edad")]
        public string Edad {  get; set; }
        [JsonProperty("Sexo")]
        public string Sexo { get; set; }
        [JsonProperty("Tamano")]
        public string Tamaño { get; set; }
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("PublicadorID")]
        public int PublicadorID { get; set; }
        [JsonProperty("UbicacionID")]
        public int UbicacionID { get; set; }
    }
}
