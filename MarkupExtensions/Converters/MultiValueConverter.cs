using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public class MultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0)
                throw new Exception();

            var value = values[0];
            if (values.Length > 1)
                parameter = values[1];

            return Converter.Convert(value, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
