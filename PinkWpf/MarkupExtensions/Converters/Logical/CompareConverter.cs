using System;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
    public abstract class CompareConverter : CompositeConverter
    {
        protected override bool CanHandleTargetNullValue => true;

        protected override object ConvertOverride(ConverterArgs e)
        {
            return ConvertCompare(e);
        }

        public abstract bool ConvertCompare(ConverterArgs e);
    }
}
