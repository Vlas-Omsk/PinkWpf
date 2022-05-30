using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PinkWpf.MarkupExtensions.Converters
{
    public sealed class ConverterArgs
    {
        public object[] Values { get; }
        public Type[] TargetTypes { get; }
        public object Parameter { get; set; }
        public CultureInfo Culture { get; set; }

        public ConverterArgs(object[] values, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Values = values;
            TargetTypes = targetTypes;
            Parameter = parameter;
            Culture = culture;
        }

        public IEnumerable<T> GetValues<T>()
        {
            return Values.Select((v, i) => ConvertValue<T>(v, $"{nameof(Values)}[{i}]"));
        }

        public object GetSingleValue()
        {
            return Values[0];
        }

        public T GetSingleValue<T>()
        {
            return GetValue<T>(0);
        }

        public T GetValue<T>(int index)
        {
            return ConvertValue<T>(Values[index], $"{nameof(Values)}[{index}]");
        }

        public T GetParameter<T>()
        {
            return ConvertValue<T>(Parameter, nameof(Parameter));
        }

        private T ConvertValue<T>(object value, string propertyName)
        {
            var result = ConvertHelper.ChangeAnyType<T>(value);
            if (result == null)
                throw new InvalidCastException($"Сannot convert a value of type {value.GetType()} to type {typeof(T)} in {propertyName}");
            return result;
        }
    }
}
