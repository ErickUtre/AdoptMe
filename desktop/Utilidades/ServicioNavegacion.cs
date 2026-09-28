using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public T GetVentanaContenedora<T>() where T : Window
        {
            return Window.GetWindow(_marcoPrincipal) as T;
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
