using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public class BoolComposer : MarkupExtension, IMultiValueConverter
    {
        public bool IsAll { get; set; } = true;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsAll)
                return values.All(elem => (bool)elem);
            else
                return values.Any(elem => (bool)elem);
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
