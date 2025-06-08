using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Modelo
{
    public class RespuestaLogin
    {
        public string Token { get; set; }
        public bool EsAdmin { get; set; }
        public Usuario Usuario { get; set; }
    }
}
