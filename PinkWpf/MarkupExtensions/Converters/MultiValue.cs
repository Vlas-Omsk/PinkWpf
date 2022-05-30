using System;
using System.Globalization;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class MultiValue : MultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        public override object Convert(ConverterArgs e)
        {
            if (e.Values.Length < 1 || e.Values.Length > 3)
                throw new Exception($"The number of values must be from 1 to 3, where 1 - {typeof(object)} Value, 2 - {typeof(object)} Parameter, 3 - {typeof(CultureInfo)} Culture");

            object value = null;
            object parameter = null;
            CultureInfo culture = null;

            if (e.Values.Length >= 1)
                value = e.Values[0];
            if (e.Values.Length >= 2)
                parameter = e.Values[1];
            if (e.Values.Length >= 3)
                culture = (CultureInfo)e.Values[2];

            return Converter.Convert(value, e.TargetTypes[0], parameter, culture);
        }
    }
}
