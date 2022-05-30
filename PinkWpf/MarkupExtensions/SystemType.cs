using System;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions
{
    public class SystemType : MarkupExtension
    {
        private object _parameter;

        public int Int { set { _parameter = value; } }
        public long Long { set { _parameter = value; } }
        public double Double { set { _parameter = value; } }
        public float Float { set { _parameter = value; } }
        public decimal Decimal { set { _parameter = value; } }
        public bool Bool { set { _parameter = value; } }
        public byte Byte { set { _parameter = value; } }
        public char Char { set { _parameter = value; } }
        public object Object { set { _parameter = value; } }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _parameter;
        }
    }
}
