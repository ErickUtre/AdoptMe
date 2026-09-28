using Cliente_AdoptMe.Modelo;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;

namespace Cliente_AdoptMe.SocketCliente
{
    public static class SocketCliente
    {
        private static Socket socket;
        private static bool estaConectado = false;

        public static bool EstaConectado => estaConectado;

        // Eventos
        public static event Action<object> MensajeRecibido;
        public static event Action<object> MensajeConfirmado;

        public static void Conectar(int usuarioID)
        {
            if (estaConectado)
                return;

            if (socket != null)
            {
                // Elimina listeners previos
                socket.Off("nuevo_mensaje");
                socket.Off("mensaje_confirmado");
                socket.Off(Socket.EVENT_CONNECT);
                socket.Off(Socket.EVENT_DISCONNECT);
                socket.Off(Socket.EVENT_CONNECT_ERROR);
            }

            socket = IO.Socket("http://localhost:8080");

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                estaConectado = true;
                Console.WriteLine("Conectado al servidor de sockets.");
                socket.Emit("unirse", usuarioID);
            });

            socket.On("nuevo_mensaje", (data) =>
            {
                try
                {
                    MensajeRecibido?.Invoke(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al procesar nuevo_mensaje: " + ex.Message);
                }
            });

            socket.On("mensaje_confirmado", (data) =>
            {
                Console.WriteLine("Mensaje confirmado: " + data);
                MensajeConfirmado?.Invoke(data);
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                estaConectado = false;
                Console.WriteLine("Desconectado del servidor de sockets.");
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, (error) =>
            {
                Console.WriteLine("Error de conexión al servidor de sockets: " + error);
            });
        }

        public static void EnviarMensaje(int remitenteID, int destinatarioID, string contenido)
        {
            if (socket != null && estaConectado)
            {
                var mensaje = new
                {
                    RemitenteID = remitenteID,
                    DestinatarioID = destinatarioID,
                    Contenido = contenido
                };

                string json = JsonConvert.SerializeObject(mensaje);

                // ✅ Envía un string JSON que el servidor puede interpretar correctamente
                socket.Emit("enviar_mensaje", json);
            }
        }



        public static void Desconectar()
        {
            if (socket != null)
            {
                socket.Off(); // Limpia todos los eventos registrados
                socket.Disconnect();
                socket = null;
            }

            estaConectado = false;
        }
    }
}
