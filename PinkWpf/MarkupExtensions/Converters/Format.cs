using System;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class Format : CompositeMultiConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return string.Format(e.GetParameter<string>(), e.Values);
        }
    }
}
