using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class ResultadoHttp
    {
        public bool Exito { get; set; }
        public HttpResponseMessage Respuesta { get; set; }
        public string MensajeError { get; set; }
        public HttpStatusCode Codigo { get; set; }
    }
}
