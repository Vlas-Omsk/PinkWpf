using System;
using System.Linq;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class If : CompositeMultiConverter
    {
        public ManyConverter ManyConverter { get; set; }

        public bool WithElse { get; set; } = false;

        protected override object ConvertOverride(ConverterArgs e)
        {
            var minimumValuesCount = WithElse ? 3 : 2;
            if (e.Values.Length < minimumValuesCount)
                throw new Exception($"Must specify at least {minimumValuesCount} values");

            var values = e.Values.Reverse();
            var valuesToComare = values.Skip(WithElse ? 2 : 1);
            bool result;

            if (ManyConverter == null)
                throw new NullReferenceException("Must specify a ManyConverter property");
            result = (bool)ManyConverter.Convert(valuesToComare.ToArray(), e.TargetTypes[0], e.Parameter, e.Culture);

            var ifResult = values.Skip(1).First();
            object elseResult = WithElse ? values.First() : null;

            return result ? ifResult : elseResult;
        }
    }
}
