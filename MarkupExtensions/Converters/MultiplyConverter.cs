using System;
using System.Globalization;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(double), typeof(double), ParameterType = typeof(double))]
    public class MultiplyConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * (double)parameter;
        }
    }
}
