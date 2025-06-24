using Cliente_AdoptMe.Grpc.ServiciosGrpc;
using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using MultimediaGrpc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Cliente_AdoptMe.Utilidades.InterfazUsuarioHelper;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ConsultarUsuario.xaml
    /// </summary>
    public partial class ConsultarUsuario : Page
    {
        public event EventHandler EventoActualizarFotoPerfil;
        public event EventHandler EventoActualizarNombre;
        private readonly MenuPrincipalUsuario _menuPrincipalUsuario = NavegadorPrincipal.Instancia.GetVentanaContenedora<MenuPrincipalUsuario>();

        public ConsultarUsuario()
        {
            InitializeComponent();
            InicializarDatosUsuario();
        }

        private void BtnExpandirImagen(object sender, RoutedEventArgs e)
        {
            ImagenExpandida imagenExpandida = new ImagenExpandida(Foto);
            imagenExpandida.ShowDialog();
        }

        private async void InicializarDatosUsuario()
        {
            Txt_Nombre.Text += $": {UsuarioSingleton.Instancia.UsuarioActual.Nombre}";
            Txt_Correo.Text += $": {UsuarioSingleton.Instancia.UsuarioActual.Acceso.Correo}";
            Txt_Telefono.Text += $": {UsuarioSingleton.Instancia.UsuarioActual.Telefono}";
            Console.WriteLine(UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil);

            if (UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil != null)
            {
                FotoComplemento.Source = UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil;
            }
            else
            {
                await MostrarFotoAsync();
            }

            if (UsuarioSingleton.Instancia.UsuarioActual.Ubicacion != null)
            {
                Txt_Ubicacion.Text = UsuarioSingleton.Instancia.UsuarioActual.Ubicacion.ToString();
            }
        }

        private async void Btn_EditarNombre(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(Properties.Resources.global_Nombre);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                UsuarioServicios usuarioServicios = new UsuarioServicios();
                string nuevoNombre = editarCampo.NuevoValor;

                _menuPrincipalUsuario?.MostrarOverlay();
                ResultadoHttp resultadoHttp = await usuarioServicios.ActualizarPerfilAsync(
                    nuevoNombre,
                    null,
                    UsuarioSingleton.Instancia.Token
                );
                _menuPrincipalUsuario.OcultarOverlay();

                if (resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        Properties.Resources.mensaje_PerfilActualizado,
                        Properties.Resources.global_Exito,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    UsuarioSingleton.Instancia.UsuarioActual.Nombre = nuevoNombre;
                    Txt_Nombre.Text = $"{Properties.Resources.global_Nombre}: {nuevoNombre}";
                }
                else
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
        
        private async void Btn_EditarCorreo(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(Properties.Resources.global_Correo);
            bool? resultado = editarCampo.ShowDialog();
            
            if (resultado == true)
            {
                AccesoServicios accesoServicios = new AccesoServicios();
                string nuevoCorreo = editarCampo.NuevoValor;

                _menuPrincipalUsuario?.MostrarOverlay();
                ResultadoHttp resultadoHttp = await accesoServicios.ActualizarAccesoAsync(
                    nuevoCorreo,
                    UsuarioSingleton.Instancia.Token
                );
                _menuPrincipalUsuario.OcultarOverlay();

                if (resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        Properties.Resources.mensaje_PerfilActualizado,
                        Properties.Resources.global_Exito,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    UsuarioSingleton.Instancia.UsuarioActual.Acceso.Correo = nuevoCorreo;
                    Txt_Correo.Text = $"{Properties.Resources.global_Correo}: {nuevoCorreo}";
                }
                else
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
        
        private async void Btn_EditarTelefono(object sender, RoutedEventArgs e)
        {
            EditarCampo editarCampo = new EditarCampo(Properties.Resources.global_Telefono);
            bool? resultado = editarCampo.ShowDialog();

            if (resultado == true)
            {
                UsuarioServicios usuarioServicios = new UsuarioServicios();
                string nuevoTelefono = editarCampo.NuevoValor;

                _menuPrincipalUsuario?.MostrarOverlay();
                ResultadoHttp resultadoHttp = await usuarioServicios.ActualizarPerfilAsync(
                    null,
                    nuevoTelefono,
                    UsuarioSingleton.Instancia.Token
                );
                _menuPrincipalUsuario.OcultarOverlay();

                if (resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        Properties.Resources.mensaje_PerfilActualizado,
                        Properties.Resources.global_Exito,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    UsuarioSingleton.Instancia.UsuarioActual.Telefono = nuevoTelefono;
                    Txt_Telefono.Text = $"{Properties.Resources.global_Telefono}: {nuevoTelefono}";
                }
                else
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private async void Btn_EditarUbicacion(object sender, RoutedEventArgs e)
        {
            if (UsuarioSingleton.Instancia.UsuarioActual.Ubicacion == null)
            {
                MessageBoxResult respuesta = MessageBox.Show(
                    Properties.Resources.mensaje_PermitirUbicacion,
                    Properties.Resources.titulo_PermisosUbicacion,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (respuesta == MessageBoxResult.No) return;
            }

            MapaRegistro mapaRegistro = new MapaRegistro()
            {
                Owner = _menuPrincipalUsuario,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            bool? resultado = mapaRegistro.ShowDialog();

            if (resultado == true)
            {
                UbicacionServicios ubicacionServicios = new UbicacionServicios();
                Ubicacion nuevaUbicacion = mapaRegistro.ResultadoUbicacion;
                _menuPrincipalUsuario?.MostrarOverlay();
                ResultadoHttp resultadoHttp = await ubicacionServicios.ActualizarUbicacionAsync(
                    nuevaUbicacion,
                    UsuarioSingleton.Instancia.Token
                );
                _menuPrincipalUsuario.OcultarOverlay();

                if (resultadoHttp.Exito)
                {
                    MessageBox.Show(
                        Properties.Resources.mensaje_PerfilActualizado,
                        Properties.Resources.global_Exito,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    UsuarioSingleton.Instancia.UsuarioActual.Ubicacion = nuevaUbicacion;
                    Txt_Ubicacion.Text = UsuarioSingleton.Instancia.UsuarioActual.Ubicacion.ToString();
                }
                else
                {
                    MessageBox.Show(
                        resultadoHttp.MensajeError,
                        Properties.Resources.global_ErrorServidor,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private async void Btn_EditarFoto(object sender, RoutedEventArgs e)
        {
            string rutaArchivo;
            
            var dialogo = new OpenFileDialog();
            dialogo.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dialogo.ShowDialog() == true)
            {
                rutaArchivo = dialogo.FileName;

                if (!File.Exists(rutaArchivo))
                {
                    MessageBox.Show("El archivo no existe.");
                    return;
                }

                await SubirArchivoAsync(rutaArchivo);
                await Task.Delay(1000);
                await MostrarFotoAsync();
            }
            else
            {
                MessageBox.Show("Seleccione un archivo primero.");
                return;
            }
        }

        private async Task MostrarFotoAsync()
        {
            EventoActualizarFotoPerfil?.Invoke(this, EventArgs.Empty);
            var imagen = await ObtenerFotoPerfilAsync(UsuarioSingleton.Instancia.Token);
            if (imagen != null)
            {
                FotoComplemento.Source = imagen;
                UsuarioSingleton.Instancia.UsuarioActual.FotoPerfil = imagen;
            }
        }

        private async Task SubirArchivoAsync(string rutaArchivo)
        {
            try
            {
                ServicioMultimediaGrpc servicioMultimedia = new ServicioMultimediaGrpc();
                await servicioMultimedia.SubirArchivoAsync(
                            rutaArchivo,
                            UsuarioSingleton.Instancia.UsuarioActual.UsuarioId,
                            UsuarioSingleton.Instancia.Token,
                            metadata => servicioMultimedia.Cliente.SubirFotoUsuario(metadata),
                            new[] { ".jpg", ".jpeg", ".png" }
                        );

            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Registro.Error($"Excepción: {ex.Message}\nTraza: {ex.StackTrace}");
                MessageBox.Show(
                    Properties.Resources.mensaje_ErrorServidor,
                    Properties.Resources.global_ErrorServidor,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
