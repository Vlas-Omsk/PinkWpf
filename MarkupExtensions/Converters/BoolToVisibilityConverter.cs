using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(string))]
    public class BoolToVisibilityConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nonVisible = Visibility.Collapsed;
            if (parameter != null)
                nonVisible = (Visibility)Enum.Parse(typeof(Visibility), parameter.ToString());
            return (bool)value ? Visibility.Visible : nonVisible;
        }
    }
}
