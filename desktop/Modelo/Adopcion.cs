using Newtonsoft.Json;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cliente_AdoptMe.Modelo
{
    public class Adopcion
    {
        [JsonProperty("AdopcionID")]
        public int AdopcionID { get; set; }

        [JsonProperty("FechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }

        [JsonProperty("Estado")]
        public bool Estado { get; set; }

        [JsonProperty("MascotaID")]
        public int MascotaID { get; set; }

        [JsonProperty("PublicadorID")]
        public int PublicadorID { get; set; }

        [JsonProperty("UbicacionID")]
        public int UbicacionID { get; set; }  

        [JsonProperty("Ubicacion")]
        public Ubicacion Ubicacion { get; set; }

        [JsonProperty("Mascota")]
        public Mascota Mascota { get; set; }

        public string Nombre => Mascota?.Nombre ?? "";
        public string Especie => Mascota?.Especie ?? "";
        public string Raza => Mascota?.Raza ?? "";
        public string Edad => Mascota?.Edad ?? "";
        public string Sexo => Mascota?.Sexo ?? "";
        public string Tamaño => Mascota?.Tamaño ?? "";
        public string Descripcion => Mascota?.Descripcion ?? "";

        public string EstadoTexto { get; set; }
        public Brush ColorEstado { get; set; }
        public ImageSource Foto { get; set; }
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
