using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public static class Validador
    {
        private static readonly Regex _nombreRegex = new Regex(
            @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+(?:\s[a-zA-ZñÑáéíóúÁÉÍÓÚ'’-]+)*$", 
            RegexOptions.None, 
            TimeSpan.FromMilliseconds(1000));
        private static readonly Regex _correoRegex = new Regex(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(1000));
        private static readonly Regex _numeroTelefonicoRegex = new Regex(
            @"^[0-9]{10}$", 
            RegexOptions.None, 
            TimeSpan.FromMilliseconds(1000));


        public static bool ValidarPatronRegex(string datos, Regex regex)
        {
            bool esValido = false;
            try
            {
                esValido = regex.IsMatch(datos);
            }
            catch (RegexMatchTimeoutException)
            {
                esValido = false;
            }
            return esValido;
        }

        public static bool ValidarNombre(string nombre)
        {
            bool esValido = false;
            string nombreLimpio = Regex.Replace(nombre.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(nombreLimpio) && ValidarPatronRegex(nombreLimpio, _nombreRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarCorreo(string correo)
        {
            bool esValido = false;
            string correoLimpio = Regex.Replace(correo.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(correoLimpio) && ValidarPatronRegex(correoLimpio, _correoRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarTelefono(string telefono)
        {
            bool esValido = false;
            string telefonoLimpio = Regex.Replace(telefono.Trim(), @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            if (!string.IsNullOrWhiteSpace(telefonoLimpio) && ValidarPatronRegex(telefonoLimpio, _numeroTelefonicoRegex))
            {
                esValido = true;
            }
            return esValido;
        }

        public static bool ValidarContraseña(string contraseña)
        {

            if (contraseña.Contains(" "))
            {
                return false;
            }

            if (contraseña.Length < 8)
            {
                return false;
            }

            if (!contraseña.Any(Char.IsUpper))
            {
                return false;
            }

            if (contraseña.Count(Char.IsDigit) < 2)
            {
                return false;
            }

            return true;
        }

        public static bool EsMismaContraseña(string contraseña, string contraseñaConfirmacion)
        {
            if (contraseña.Equals(contraseñaConfirmacion))
            {
                return true;
            }
            return false;
        }
    }
}
