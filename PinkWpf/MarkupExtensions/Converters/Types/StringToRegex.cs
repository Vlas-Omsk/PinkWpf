using System;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(string), typeof(Regex))]
    public sealed class StringToRegex : CompositeConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return new Regex(e.GetSingleValue<string>());
        }
    }
}
