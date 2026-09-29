using System.Xml.Linq;

namespace AdoptMe.Escritorio.Nucleo.Pruebas;

public class RecursosXamlPruebas
{
    private static readonly XNamespace EspacioXaml = "http://schemas.microsoft.com/winfx/2006/xaml";

    [Fact]
    public void LasClavesDeLosDiccionariosCompartidosSonUnicas()
    {
        var diccionarios = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Estilos"), "*.xaml");

        var duplicadas = diccionarios
            .SelectMany(ruta => XDocument.Load(ruta).Root!.Elements()
                .Select(recurso => recurso.Attribute(EspacioXaml + "Key")?.Value)
                .OfType<string>()
                .Select(clave => (Clave: clave, Archivo: Path.GetFileName(ruta))))
            .GroupBy(recurso => recurso.Clave)
            .Where(grupo => grupo.Count() > 1)
            .Select(grupo => $"{grupo.Key}: {string.Join(", ", grupo.Select(recurso => recurso.Archivo))}")
            .ToList();

        Assert.NotEmpty(diccionarios);
        Assert.Empty(duplicadas);
    }
}
