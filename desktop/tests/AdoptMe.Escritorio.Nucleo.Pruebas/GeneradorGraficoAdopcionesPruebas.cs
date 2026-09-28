using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Reportes;
using OxyPlot.Series;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class GeneradorGraficoAdopcionesPruebas
{
    [Fact]
    public void RellenaLosMesesSinAdopcionesEntreElPrimeroYElUltimo()
    {
        var adopciones = new[]
        {
            new AdopcionResumen(1, true, new DateTime(2025, 1, 10)),
            new AdopcionResumen(2, true, new DateTime(2025, 1, 20)),
            new AdopcionResumen(3, true, new DateTime(2025, 4, 5)),
            new AdopcionResumen(4, true, null)
        };

        var conteos = GeneradorGraficoAdopciones.ContarPorMes(adopciones);

        Assert.Equal([2, 0, 0, 1], conteos.Select(conteo => conteo.Cantidad));
        Assert.Equal(new DateOnly(2025, 1, 1), conteos[0].Mes);
        Assert.Equal(new DateOnly(2025, 4, 1), conteos[^1].Mes);
    }

    [Fact]
    public void DevuelveUnaListaVaciaSinFechas() =>
        Assert.Empty(GeneradorGraficoAdopciones.ContarPorMes([new AdopcionResumen(1, false, null)]));

    [Fact]
    public void GeneraUnaBarraPorMes()
    {
        var conteos = new[] { new ConteoMensual(new DateOnly(2025, 1, 1), 3), new ConteoMensual(new DateOnly(2025, 2, 1), 5) };

        var modelo = GeneradorGraficoAdopciones.Generar("Reporte", conteos);

        var serie = Assert.IsType<BarSeries>(Assert.Single(modelo.Series));
        Assert.Equal([3d, 5d], serie.ItemsSource.Cast<BarItem>().Select(barra => barra.Value));
    }
}
