using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public abstract class CompositeConverter : MarkupExtension, IValueConverter, IUniversalConverter
    {
        public IUniversalConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public CultureInfo ConverterCulture { get; set; }
        public object TargetNullValue { get; set; }
        public bool? PassParameter { get; set; }
        public bool? PassCulture { get; set; }

        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = new ConverterArgs(new[] { value }, new[] { targetType }, parameter, culture);
            return Convert(args);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUniversalConverter
        public object Convert(ConverterArgs e)
        {
            if (!CanHandleTargetNullValue && (e.Values.Length == 0 || e.Values.All(v => v == null)))
                return TargetNullValue;

            var value = ConvertOverride(e);

            if (Converter != null)
            {
                var args = new ConverterArgs(new[] { value }, e.TargetTypes, e.Parameter, e.Culture);
                if (PassParameter ?? ConverterParameter != null)
                    args.Parameter = ConverterParameter;
                if (PassCulture ?? PassCulture != null)
                    args.Parameter = ConverterCulture;
                value = Converter.Convert(args);
            }

            return value;
        }

        public object[] ConvertBack(ConverterArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected virtual bool CanHandleTargetNullValue => false;

        protected abstract object ConvertOverride(ConverterArgs e);

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
