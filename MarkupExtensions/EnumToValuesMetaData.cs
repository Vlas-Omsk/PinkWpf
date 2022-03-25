using System;
using System.Linq;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class EnumToValuesMetaData : MarkupExtension
    {
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return EnumHelper.GetValuesMetaData(EnumType).Select(k => k.Value);
        }
    }
}
