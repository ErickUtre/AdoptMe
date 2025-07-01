using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cliente_AdoptMe.Conversores
{
    public class BooleanToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool esPropio = (bool)value;
            return esPropio ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
