using System;
using System.Globalization;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(Enum), typeof(ValueMetaData))]
    public sealed class EnumValueToValueMetaData : ValueConverter
    {
        public override object Convert(ConverterArgs e)
        {
            var value = e.GetSingleValue();
            return EnumHelper.GetValueMetaData(value.GetType(), value);
        }

        public override object[] ConvertBack(ConverterArgs e)
        {
            return new[] { EnumHelper.GetEnumValue(e.TargetTypes[0], e.GetSingleValue<ValueMetaData>()) };
        }
    }
}
