using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(IDictionary), typeof(object), ParameterType = typeof(object))]
    public class DictionaryConverter : MarkupExtension, IValueConverter
    {
        private static DictionaryConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = (IDictionary)value;
            if (dictionary.Contains(parameter))
                return dictionary[parameter];
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new DictionaryConverter());
        }
    }
}
