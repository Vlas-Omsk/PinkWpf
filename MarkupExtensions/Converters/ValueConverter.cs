using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        public IValueConverter Converter { get; set; }
        public CultureInfo ConverterCulture { get; set; }
        public object ConverterParameter { get; set; }
        public object TargetNullValue { get; set; }
        public bool PassParameter { get; set; } = false;
        public bool PassCulture { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return TargetNullValue;
            value = ConvertInternal(value, targetType, parameter, culture);
            if (value == null)
                return TargetNullValue;
            if (Converter != null)
            {
                if (!PassParameter)
                    parameter = ConverterParameter;
                if (!PassCulture)
                    culture = ConverterCulture;
                value = Converter.Convert(value, targetType, parameter, culture);
            }
            if (value == null)
                return TargetNullValue;
            return value;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected abstract object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
