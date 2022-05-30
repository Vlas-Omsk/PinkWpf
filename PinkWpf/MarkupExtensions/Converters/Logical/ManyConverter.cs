using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
    public abstract class ManyConverter : CompositeMultiConverter
    {
        public CompareConverter CompareConverter { get; set; }

        protected override bool CanHandleTargetNullValue => true;

        protected sealed override object ConvertOverride(ConverterArgs e)
        {
            return LogicalConvert(e.Values.Select(o => (bool)CompareConverter.Convert(o, e.TargetTypes[0], e.Parameter, e.Culture)));
        }

        protected abstract bool LogicalConvert(IEnumerable<bool> values);
    }
}
