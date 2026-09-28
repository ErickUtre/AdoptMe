using Cliente_AdoptMe.Modelo;
using Cliente_AdoptMe.Servicios;
using Cliente_AdoptMe.Utilidades;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para ReporteMascotasAdoptadas.xaml
    /// </summary>
    
    public enum TipoReporte { Pendientes, Aceptadas }
    public partial class Reporte : Page
    {
        public TipoReporte _tipoReporte;
        public Reporte(TipoReporte tipoReporte)
        {
            InitializeComponent();
            _tipoReporte = tipoReporte;
            Loaded += Reporte_Loaded;
        }

        private async void Reporte_Loaded(object sender, RoutedEventArgs e)
        {
            switch (_tipoReporte)
            {
                case TipoReporte.Pendientes:
                    await MostrarPendientes();
                    break;
                case TipoReporte.Aceptadas:
                    await MostrarAceptadas();
                    break;
            }
        }

        private async Task MostrarPendientes()
        {
            var servicio = new AdopcionServicios();
            var datos = await ProcesarConteoPorMesAsync(servicio.ObtenerAdopcionesPendientesAsync());
            plotView.Model = GeneradorReportes.GenerarGraficoAdopciones("Reporte de Mascotas en Adopción", datos);
        }

        private async Task MostrarAceptadas()
        {
            var servicio = new AdopcionServicios();
            var datos = await ProcesarConteoPorMesAsync(servicio.ObtenerAdopcionesAceptadasAsync());
            plotView.Model = GeneradorReportes.GenerarGraficoAdopciones("Reporte de Mascotas Adoptadas", datos);
        }

        private async Task<Dictionary<string, int>> ProcesarConteoPorMesAsync(Task<ResultadoHttp> tareaHttp)
        {
            var resultado = await tareaHttp;

            if (!resultado.Exito)
            {
                MessageBox.Show(
                    resultado.MensajeError,
                    Properties.Resources.global_ErrorServidor,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return new Dictionary<string, int>();
            }

            var json = await resultado.Respuesta.Content.ReadAsStringAsync();

            var listaAdopciones = JsonSerializer.Deserialize<List<AdopcionDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var grupos = listaAdopciones
                .Where(a => a.FechaSolicitud.HasValue)
                .GroupBy(a => new
                {
                    Año = a.FechaSolicitud.Value.Year,
                    Mes = a.FechaSolicitud.Value.Month
                })
                .OrderBy(g => g.Key.Año)
                .ThenBy(g => g.Key.Mes)
                .ToList();

            var datosPorMes = new Dictionary<string, int>();
            var cultura = new System.Globalization.CultureInfo("es-MX");

            foreach (var grupo in grupos)
            {
                var fechaRepresentativa = new DateTime(grupo.Key.Año, grupo.Key.Mes, 1);
                string clave = fechaRepresentativa.ToString("MMMM yyyy", cultura);
                datosPorMes[clave] = grupo.Count();
            }

            return datosPorMes;
        }

        private void Btn_Descargar(object sender, RoutedEventArgs e)
        {
            if (plotView.Model == null)
            {
                MessageBox.Show("No hay gráfico para exportar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog dialogoGuardar = new SaveFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                FileName = "reporte_adopciones.pdf"
            };

            if (dialogoGuardar.ShowDialog() == true)
            {
                string rutaArchivo = dialogoGuardar.FileName;

                using (var stream = File.Create(rutaArchivo))
                {
                    var pdfExporter = new OxyPlot.SkiaSharp.PdfExporter
                    {
                        Width = 800,
                        Height = 600,
                    };

                    pdfExporter.Export(plotView.Model, stream);
                }

                MessageBox.Show("PDF generado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
