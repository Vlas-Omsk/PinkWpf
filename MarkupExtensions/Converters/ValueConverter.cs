using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public abstract class ValueConverter : MarkupExtension, IValueConverter, IUniversalConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(new[] { value }, new Type[] { targetType }, parameter, culture);
            return Convert(args);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(new[] { value }, new Type[] { targetType }, parameter, culture);
            return ConvertBack(args)[0];
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
