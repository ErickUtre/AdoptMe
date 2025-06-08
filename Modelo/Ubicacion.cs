using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class Ubicacion
    {
        [JsonProperty("UbicacionID")]
        public int? UbicacionID { get; set; }
        [JsonProperty("Ciudad")]
        public string Ciudad {  get; set; }
        [JsonProperty("Estado")]
        public string Estado { get; set; }
        [JsonProperty("Pais")]
        public string Pais { get; set; }
        [JsonProperty("Longitud")]
        public double Longitud {  get; set; }
        [JsonProperty("Latitud")]
        public double Latitud { get; set; }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Ciudad: {Ciudad}");
            sb.AppendLine($"Estado: {Estado}");
            sb.AppendLine($"País: {Pais}");
            sb.AppendLine($"Latitud: {Latitud}");
            sb.AppendLine($"Longitud: {Longitud}");

            return sb.ToString();
        }
    }
}
