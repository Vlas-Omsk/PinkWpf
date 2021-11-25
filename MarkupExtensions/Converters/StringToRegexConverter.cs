using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(Regex))]
    public class StringToRegexConverter : MarkupExtension, IValueConverter
    {
        private static StringToRegexConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Regex(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new StringToRegexConverter());
        }
    }
}
