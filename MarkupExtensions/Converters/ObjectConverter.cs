using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(object), typeof(object), ParameterType = typeof(string))]
    public class ObjectConverter : MarkupExtension, IValueConverter
    {
        private static ObjectConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var propertyInfo = value.GetType().GetProperty(parameter.ToString());
            return propertyInfo.GetValue(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ObjectConverter());
        }
    }
}
