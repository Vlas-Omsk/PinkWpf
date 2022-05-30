using System;
using System.Collections.Generic;
using System.Linq;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class All : ManyConverter
    {
        protected override bool LogicalConvert(IEnumerable<bool> values)
        {
            return values.All(b => b);
        }
    }
}
