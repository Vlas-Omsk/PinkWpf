using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public abstract class MultiValueConverter : MarkupExtension, IMultiValueConverter, IUniversalConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(values, new[] { targetType }, parameter, culture);
            return Convert(args);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(new[] { value }, targetTypes, parameter, culture);
            return ConvertBack(args);
        }

        #region IUniversalConverter
        public virtual object Convert(ConverterArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual object[] ConvertBack(ConverterArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
