using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_AdoptMe.Utilidades
{
    public static class GeneradorReportes
    {
        public static PlotModel GenerarGraficoAdopciones(string titulo, Dictionary<string, int> datosPorMes)
        {
            var modelo = new PlotModel { Title = titulo };

            var meses = new string[]
            {
                "enero", "febrero", "marzo", "abril", "mayo", "junio",
                "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
            };

            var grupos = datosPorMes
                .GroupBy(kvp => kvp.Key.Split(' ')[1])
                .ToDictionary(g => g.Key, g => g.ToDictionary(k => k.Key.Split(' ')[0].ToLower(), k => k.Value));

            var datosCompletos = new Dictionary<string, int>();

            foreach (var año in grupos.Keys.OrderBy(a => int.Parse(a)))
            {
                var datosMeses = grupos[año];
                foreach (var mes in meses)
                {
                    string clave = $"{CulturaPrimeraLetraMayus(mes)} {año}";
                    datosCompletos[clave] = datosMeses.ContainsKey(mes) ? datosMeses[mes] : 0;
                }
            }

            var categorias = datosCompletos.Keys.ToList();
            var valores = datosCompletos.Values.ToList();

            modelo.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = datosPorMes.Keys.ToList(),
                Angle = 45,
                Title = "Mes"
            });

            modelo.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Title = "Cantidad"
            });

            var series = new BarSeries
            {
                FillColor = OxyColors.SkyBlue
            };

            series.ItemsSource = datosPorMes.Values.Select(v => new BarItem { Value = v }).ToList();

            modelo.Series.Add(series);

            return modelo;
        }

        private static string CulturaPrimeraLetraMayus(string texto)
        {
            return char.ToUpper(texto[0]) + texto.Substring(1);
        }
    }
}
