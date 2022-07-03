using System;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class IsNullOrEmpty : CompareConverter
    {
        public override bool ConvertCompare(ConverterArgs e)
        {
            return string.IsNullOrEmpty(e.GetSingleValue<string>());
        }
    }
}
