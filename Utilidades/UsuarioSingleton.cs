using Cliente_AdoptMe.Grpc.ServiciosGrpc;
using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.SocketCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public class UsuarioSingleton
    {
        private static readonly Lazy<UsuarioSingleton> lazy =
            new Lazy<UsuarioSingleton>(() => new UsuarioSingleton());

        public static UsuarioSingleton Instancia => lazy.Value;

        public Usuario UsuarioActual { get; private set; }
        public string Token { get; private set; }
        public ServicioNotificacionGrpc ServicioNotificacion { get; set; }


        private UsuarioSingleton() { }

        public void IniciarSesion(Usuario usuario, string token)
        {
            UsuarioActual = usuario;
            Token = token;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            Token = null;
        }

        public bool EstaAutenticado()
        {
            return !string.IsNullOrEmpty(Token);
        }
    }
}
