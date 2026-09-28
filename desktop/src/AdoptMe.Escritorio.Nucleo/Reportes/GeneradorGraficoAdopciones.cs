using System.Globalization;
using AdoptMe.Escritorio.Nucleo.Modelos;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace AdoptMe.Escritorio.Nucleo.Reportes;

public sealed record ConteoMensual(DateOnly Mes, int Cantidad)
{
    public string Etiqueta(CultureInfo cultura) =>
        cultura.TextInfo.ToTitleCase(Mes.ToString("MMMM yyyy", cultura));
}

public interface IExportadorReportes
{
    void ExportarPdf(PlotModel grafico, string ruta);
}

public static class GeneradorGraficoAdopciones
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-MX");

    public static IReadOnlyList<ConteoMensual> ContarPorMes(IEnumerable<AdopcionResumen> adopciones)
    {
        var fechas = adopciones
            .Where(adopcion => adopcion.FechaSolicitud.HasValue)
            .Select(adopcion => new DateOnly(adopcion.FechaSolicitud!.Value.Year, adopcion.FechaSolicitud.Value.Month, 1))
            .ToList();
        if (fechas.Count == 0)
        {
            return [];
        }

        var conteos = fechas.GroupBy(mes => mes).ToDictionary(grupo => grupo.Key, grupo => grupo.Count());
        var resultado = new List<ConteoMensual>();
        for (var mes = fechas.Min(); mes <= fechas.Max(); mes = mes.AddMonths(1))
        {
            resultado.Add(new ConteoMensual(mes, conteos.GetValueOrDefault(mes)));
        }
        return resultado;
    }

    public static PlotModel Generar(string titulo, IReadOnlyList<ConteoMensual> conteos)
    {
        ArgumentNullException.ThrowIfNull(conteos);
        var modelo = new PlotModel { Title = titulo };
        modelo.Axes.Add(new CategoryAxis
        {
            Position = AxisPosition.Left,
            Title = Textos.Mes,
            ItemsSource = conteos.Select(conteo => conteo.Etiqueta(Cultura)).ToList()
        });
        modelo.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Minimum = 0,
            MinimumMajorStep = 1,
            Title = Textos.Cantidad
        });
        modelo.Series.Add(new BarSeries
        {
            FillColor = OxyColors.SkyBlue,
            LabelPlacement = LabelPlacement.Inside,
            LabelFormatString = "{0}",
            ItemsSource = conteos.Select(conteo => new BarItem { Value = conteo.Cantidad }).ToList()
        });
        return modelo;
    }
}
