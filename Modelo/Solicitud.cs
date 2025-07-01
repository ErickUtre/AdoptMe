using Newtonsoft.Json;

public class Solicitud
{
    [JsonProperty("SolicitudID")]
    public int SolicitudID { get; set; }

    [JsonProperty("AdopcionID")]
    public int AdopcionID { get; set; }

    [JsonProperty("NombreAdoptante")]
    public string NombreAdoptante { get; set; }

    [JsonProperty("AdoptanteID")]
    public int AdoptanteID { get; set; }
}
