using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cliente_AdoptMe.Utilidades
{
    public class ServicioNavegacion
    {
        private Frame _marcoPrincipal;

        public void SetMarco(Frame marco)
        {
            _marcoPrincipal = marco;
        }

        public Frame GetMarco()
        {
            return _marcoPrincipal;
        }

        public void Navegar(object marco)
        {
            _marcoPrincipal?.Navigate(marco);
        }

        public void Regresar()
        {
            if (_marcoPrincipal?.CanGoBack == true)
            {
                _marcoPrincipal.GoBack();
            }
        }

        public void Avanzar()
        {
            if (_marcoPrincipal?.CanGoForward == true)
            {
                _marcoPrincipal.GoForward();
            }
        }

        public Page GetContenido()
        {
            return _marcoPrincipal.NavigationService.Content as Page;
        }
    }
}
