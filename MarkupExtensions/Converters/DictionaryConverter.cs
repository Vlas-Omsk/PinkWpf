using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(IDictionary), typeof(object), ParameterType = typeof(object))]
    public class DictionaryConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = (IDictionary)value;
            if (dictionary.Contains(parameter))
                return dictionary[parameter];
            return null;
        }
    }
}
