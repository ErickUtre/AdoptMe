using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class MensajeEnviar
    {
        public int RemitenteID { get; set; }
        public int DestinatarioID { get; set; }
        public string Contenido { get; set; }
    }
}

