using System;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class Invert : CompositeConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return !e.GetSingleValue<bool>();
        }
    }
}
