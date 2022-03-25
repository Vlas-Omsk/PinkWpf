using System;
using System.Globalization;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    public abstract class CompositeMultiConverter : CompositeConverter, IMultiValueConverter
    {
        #region IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(values, new[] { targetType }, parameter, culture);
            return Convert(args);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
