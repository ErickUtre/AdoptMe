using Newtonsoft.Json;

namespace Cliente_AdoptMe.Modelo
{
    public class Chat
    {
        [JsonProperty("UsuarioID")]
        public int UsuarioID { get; set; }

        [JsonProperty("Nombre")]
        public string Nombre { get; set; }

        [JsonProperty("UltimoMensaje")]
        public string UltimoMensaje { get; set; }
    }
}
