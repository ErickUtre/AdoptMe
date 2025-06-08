using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public static class Encriptador
    {
        public static string GenerarHashSHA512(string contraseña)
        {
            var contraseñaHasheada = "";
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                contraseñaHasheada = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }

            return contraseñaHasheada;
        }
    }
}
