using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Logica
{
    public class Mascota
    {
        public int MascotaID { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public string Edad { get; set; } 
        public string Sexo { get; set; } 
        public string Tamaño { get; set; } 
        public string Descripcion { get; set; }
        public bool EstadoAdopcion { get; set; } 
        public DateTime FechaPublicacion { get; set; }
        public int PublicadorID { get; set; }
    }
}
