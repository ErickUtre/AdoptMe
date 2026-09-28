using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cliente_AdoptMe.Conversores
{
    public class BooleanToBubbleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool esPropio = (bool)value;
            return esPropio ? new SolidColorBrush(Color.FromRgb(32, 119, 102)) : new SolidColorBrush(Color.FromRgb(169, 169, 169));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
