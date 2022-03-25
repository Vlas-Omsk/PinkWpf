using System;
using System.Linq;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public sealed class Multiply : CompositeMultiConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return e.GetValues<double>().Aggregate((left, right) => left * right) * e.GetParameter<double>();
        }
    }
}
