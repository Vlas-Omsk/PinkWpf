using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(Regex))]
    public class StringToRegexConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Regex(value.ToString());
        }
    }
}
