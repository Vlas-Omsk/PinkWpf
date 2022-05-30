using System;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class Equals : CompareConverter
    {
        public override bool ConvertCompare(ConverterArgs e)
        {
            var value = e.GetSingleValue();
            return ((value != null) && value.GetType().IsValueType)
                     ? value.Equals(e.Parameter)
                     : (value == e.Parameter);
        }
    }
}
