using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(bool))]
    public class BoolToVisibilityConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && (bool)parameter)
                value = !(bool)value;
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
