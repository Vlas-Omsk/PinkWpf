using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    public class ToColorConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ColorConverter.ConvertFromString(value.ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}
