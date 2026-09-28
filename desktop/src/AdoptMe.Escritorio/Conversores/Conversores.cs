using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdoptMe.Escritorio.Conversores;

public abstract class ConversorUnidireccional : IValueConverter
{
    public abstract object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => Binding.DoNothing;
}

public static class Imagenes
{
    public static BitmapImage? DesdeBytes(byte[]? bytes)
    {
        if (bytes is not { Length: > 0 })
        {
            return null;
        }
        using var flujo = new MemoryStream(bytes);
        var imagen = new BitmapImage();
        imagen.BeginInit();
        imagen.CacheOption = BitmapCacheOption.OnLoad;
        imagen.StreamSource = flujo;
        imagen.EndInit();
        imagen.Freeze();
        return imagen;
    }
}

public sealed class BytesAImagenConversor : ConversorUnidireccional
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Imagenes.DesdeBytes(value as byte[])
        ?? (parameter is string ruta ? new BitmapImage(new Uri(ruta, UriKind.RelativeOrAbsolute)) : null);
}

public sealed class BooleanoAVisibilidadConversor : ConversorUnidireccional
{
    public bool Invertir { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value is true) ^ Invertir ? Visibility.Visible : Visibility.Collapsed;
}

public sealed class TextoVacioAVisibilidadConversor : ConversorUnidireccional
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
}

public sealed class NuloAVisibilidadConversor : ConversorUnidireccional
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is null ? Visibility.Collapsed : Visibility.Visible;
}

public sealed class BooleanoAPinceladaConversor : ConversorUnidireccional
{
    public Brush Verdadero { get; set; } = Brushes.Transparent;

    public Brush Falso { get; set; } = Brushes.Transparent;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Verdadero : Falso;
}

public sealed class BooleanoAAlineacionConversor : ConversorUnidireccional
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? HorizontalAlignment.Right : HorizontalAlignment.Left;
}

public sealed class TipoCampoAVisibilidadConversor : ConversorUnidireccional
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        string.Equals(value?.ToString(), parameter as string, StringComparison.Ordinal) ? Visibility.Visible : Visibility.Collapsed;
}
