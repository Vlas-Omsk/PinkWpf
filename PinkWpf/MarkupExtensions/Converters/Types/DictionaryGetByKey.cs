using System;
using System.Collections;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(IDictionary), typeof(object), ParameterType = typeof(object))]
    public sealed class DictionaryGetByKey : CompositeConverter
    {
        public bool ThrowIfNotContains { get; set; } = false;

        protected override object ConvertOverride(ConverterArgs e)
        {
            var dictionary = e.GetSingleValue<IDictionary>();
            if (ThrowIfNotContains || dictionary.Contains(e.Parameter))
                return dictionary[e.Parameter];
            return null;
        }
    }
}
