using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Reportes;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Reportes;

public sealed partial class ReportesVistaModelo(INavegador navegador) : VistaModeloBase
{
    [RelayCommand]
    private Task VerAdoptadasAsync() => navegador.NavegarAAsync<ReporteVistaModelo>(EstadoAdopcion.Adoptada);

    [RelayCommand]
    private Task VerEnAdopcionAsync() => navegador.NavegarAAsync<ReporteVistaModelo>(EstadoAdopcion.Disponible);
}

public sealed partial class ReporteVistaModelo(
    IServicioAdopciones adopciones,
    IExportadorReportes exportador,
    INavegador navegador,
    IDialogos dialogos) : VistaModeloBase, IAlNavegar
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DescargarCommand))]
    private PlotModel? grafico;

    [ObservableProperty]
    private string titulo = string.Empty;

    public async Task AlNavegarAsync(object? parametro)
    {
        var estado = parametro as EstadoAdopcion? ?? EstadoAdopcion.Adoptada;
        Titulo = estado == EstadoAdopcion.Adoptada ? Textos.ReporteAdoptadas : Textos.ReporteEnAdopcion;
        var resultado = await MientrasOcupadoAsync(() => adopciones.ListarParaReporteAsync(estado)).ConfigureAwait(true);
        if (!Informar(dialogos, resultado, out var lista))
        {
            return;
        }
        var conteos = GeneradorGraficoAdopciones.ContarPorMes(lista);
        if (conteos.Count == 0)
        {
            dialogos.Mostrar(Textos.SinDatosReporte, TipoMensaje.Informacion, Textos.Aplicacion);
        }
        Grafico = GeneradorGraficoAdopciones.Generar(Titulo, conteos);
    }

    private bool PuedeDescargar() => Grafico is not null;

    [RelayCommand(CanExecute = nameof(PuedeDescargar))]
    private void Descargar()
    {
        if (Grafico is null || dialogos.SeleccionarDestino(FiltroArchivo.Pdf, "reporte_adopciones.pdf") is not { } ruta)
        {
            return;
        }
        exportador.ExportarPdf(Grafico, ruta);
        dialogos.Mostrar(Textos.ReporteGenerado, TipoMensaje.Informacion, Textos.Exito);
    }

    [RelayCommand]
    private Task RegresarAsync() => navegador.NavegarAAsync<ReportesVistaModelo>();
}
