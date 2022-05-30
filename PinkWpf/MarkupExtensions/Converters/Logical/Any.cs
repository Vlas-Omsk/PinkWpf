using System;
using System.Collections.Generic;
using System.Linq;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class Any : ManyConverter
    {
        protected override bool LogicalConvert(IEnumerable<bool> values)
        {
            return values.Any(b => b);
        }
    }
}
