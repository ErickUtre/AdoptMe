using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Logica
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
    }
}
