using System;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class ToArray : CompositeMultiConverter
    {
        public Type ElementType { get; set; }

        protected override object ConvertOverride(ConverterArgs e)
        {
            var elementType = ElementType;

            if (elementType == null)
                elementType = GetElementType(e.TargetTypes[0]);

            var array = Array.CreateInstance(elementType, e.Values.Length);
            e.Values.CopyTo(array, 0);

            return array;
        }

        private Type GetElementType(Type type)
        {
            var enumerableType = type;
            if (type.Name != "IEnumerable`1")
                enumerableType = type.GetInterface("IEnumerable`1");

            if (enumerableType == null)
                return typeof(object);
            else
                return enumerableType.GenericTypeArguments[0];
        }
    }
}
