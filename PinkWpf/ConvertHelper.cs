using System;
using System.ComponentModel;

namespace PinkWpf
{
    public static class ConvertHelper
    {
        public static T ChangeAnyType<T>(object value)
        {
            return (T)ChangeAnyType(value, typeof(T));
        }

        public static object ChangeAnyType(object value, Type targetType)
        {
            var valueType = value.GetType();
            if (valueType == targetType || targetType.IsAssignableFrom(valueType))
                return value;
            if (valueType == typeof(string))
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(targetType);
                    if (converter != null)
                        return converter.ConvertFromString((string)value);
                }
                catch (NotSupportedException)
                {
                }
            }
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }
}
