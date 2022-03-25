using System;
using System.Globalization;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class IsNullOrDefault : CompareConverter
    {
        public override bool ConvertCompare(ConverterArgs e)
        {
            var value = e.GetSingleValue();
            return value == null || value == default;
        }
    }
}
