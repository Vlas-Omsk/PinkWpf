using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public class IfConverter : MarkupExtension, IMultiValueConverter
    {
        public bool IsAll { get; set; } = true;
        public int ConditionCount { get; set; } = 1;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > ConditionCount + 2 || values.Length < ConditionCount + 1)
                throw new Exception("You must specify 2 or 3 values. ");
            var conditions = values.TakeWhile((_, i) => i < ConditionCount);
            bool condition;
            if (IsAll)
                condition = conditions.All(b => (bool)b);
            else
                condition = conditions.Any(b => (bool)b);
            if (condition)
                return values[ConditionCount];
            else if (values.Length - 1 == ConditionCount + 1)
                return values[ConditionCount + 1];
            else
                return null;
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
