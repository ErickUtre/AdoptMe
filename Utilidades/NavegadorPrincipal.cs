using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public static class NavegadorPrincipal
    {
        public static ServicioNavegacion Instancia { get; } = new ServicioNavegacion();
    }
}
