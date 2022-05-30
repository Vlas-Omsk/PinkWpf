using System;
using System.Linq;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public sealed class Add : CompositeMultiConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            return e.GetValues<double>().Sum() + e.GetParameter<double>();
        }
    }
}
