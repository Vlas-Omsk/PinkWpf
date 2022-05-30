using System;
using System.Windows.Data;
using System.Windows.Media;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(Color))]
    public sealed class StringToColor : CompositeConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return ColorConverter.ConvertFromString(e.GetSingleValue<string>());
        }
    }
}
