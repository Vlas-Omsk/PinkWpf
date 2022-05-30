using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PinkWpf
{
    public static class EnumHelper
    {
        private readonly static Dictionary<Type, Dictionary<ulong, ValueMetaData>> _enums = new Dictionary<Type, Dictionary<ulong, ValueMetaData>>();
        private readonly static MethodInfo _enumToUInt64MethodInfo;

        static EnumHelper()
        {
            _enumToUInt64MethodInfo = typeof(Enum).GetMethod("ToUInt64", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static IEnumerable<KeyValuePair<Enum, ValueMetaData>> GetValuesMetaData<T>() where T : Enum
        {
            return GetValuesMetaData(typeof(T));
        }

        public static IEnumerable<KeyValuePair<Enum, ValueMetaData>> GetValuesMetaData(Type enumType)
        {
            return enumType.GetEnumValues().Cast<Enum>().Select(e => new KeyValuePair<Enum, ValueMetaData>(e, GetValueMetaData(enumType, e)));
        }

        private static Dictionary<ulong, ValueMetaData> GetValuesMetaDataInternal(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Type is not enum");

            if (_enums.TryGetValue(enumType, out Dictionary<ulong, ValueMetaData> valuesMetaData))
                return valuesMetaData;

            valuesMetaData = new Dictionary<ulong, ValueMetaData>();

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var field in fields)
            {
                var value = GetEnumUInt64(field.GetRawConstantValue());
                var attribute = (ValueMetaData)field.GetCustomAttributes(typeof(ValueMetaData), false).FirstOrDefault();

                valuesMetaData.Add(value, attribute);
            }

            return _enums[enumType] = valuesMetaData;
        }

        public static ValueMetaData GetValueMetaData<T>(T value) where T : Enum
        {
            return GetValueMetaData(typeof(T), value);
        }

        public static ValueMetaData GetValueMetaData<T>(object value) where T : Enum
        {
            return GetValueMetaData(typeof(T), value);
        }

        public static ValueMetaData GetValueMetaData(Type enumType, object value)
        {
            var enumValue = GetEnumUInt64(value);
            GetValuesMetaDataInternal(enumType).TryGetValue(enumValue, out ValueMetaData valueMetaData);
            return valueMetaData;
        }

        public static T GetEnumValue<T>(ValueMetaData metaData) where T : Enum
        {
            return (T)GetEnumValue(typeof(T), metaData);
        }

        public static Enum GetEnumValue(Type enumType, ValueMetaData metaData)
        {
            return GetValuesMetaData(enumType).FirstOrDefault(k => k.Value == metaData).Key;
        }

        private static ulong GetEnumUInt64(object value)
        {
            return (ulong)_enumToUInt64MethodInfo.Invoke(null, new[] { value });
        }
    }
}
