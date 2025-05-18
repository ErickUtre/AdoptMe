using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cliente_AdoptMe.Utilidades
{
    public static class PasswordBoxExtensiones
    {
        public static readonly DependencyProperty PropiedadTextoSugerido =
            DependencyProperty.RegisterAttached(
                "TextoSugerido",
                typeof(string),
                typeof(PasswordBoxExtensiones),
                new FrameworkPropertyMetadata(string.Empty)
            );
        public static string GetTextoSugerido(DependencyObject obj)
        {
            return (string)obj.GetValue(PropiedadTextoSugerido);
        }

        public static void SetTextoSugerido(DependencyObject obj, string valor)
        {
            obj.SetValue(PropiedadTextoSugerido, valor);
        }
    }
}
