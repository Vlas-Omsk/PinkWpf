using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace PinkWpf.GridDefinitionsMarkup
{
    internal sealed class GridDefinitionsMarkupParser
    {
        private readonly IResourceLocator _resourceLocator;
        private readonly string _defaultResourceKey;

        public GridDefinitionsMarkupParser(IResourceLocator resourceLocator, string defaultResourceKey)
        {
            _resourceLocator = resourceLocator;
            _defaultResourceKey = defaultResourceKey;
        }

        public IEnumerable<GridDefinition> ParseDefinitions(string str)
        {
            if (string.IsNullOrEmpty(str))
                yield break;

            var split = str.Split(',');

            foreach (var item in split)
            {
                var trimmedItem = item.Trim();

                if (string.IsNullOrEmpty(trimmedItem))
                {
                    yield return new GridDefinition(false, null, null, null);
                    continue;
                }

                var isGap = false;

                if (trimmedItem[0] == '[' && trimmedItem[trimmedItem.Length - 1] == ']')
                {
                    trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
                    isGap = true;
                }

                var values = trimmedItem.Split('|');

                if (values.Length > 3 || values.Length < 1)
                    throw new Exception("values.Length > 3 || values.Length < 1");

                var size = ParseValue<GridLength?>(values[0]);
                double? minSize = null;
                double? maxSize = null;

                if (values.Length > 1)
                    minSize = ParseValue<double?>(values[1]);

                if (values.Length > 2)
                    maxSize = ParseValue<double?>(values[2]);

                yield return new GridDefinition(isGap, size, minSize, maxSize);
            }
        }

        private T ParseValue<T>(string str)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
                return default;

            var isResource = false;

            if (str[0] == '{' && str[str.Length - 1] == '}')
            {
                str = str.Substring(1, str.Length - 2);
                isResource = true;
            }

            if (isResource)
            {
                if (string.IsNullOrEmpty(str))
                    str = _defaultResourceKey;

                return (T)_resourceLocator.FindResource(str);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromInvariantString(str);
        }
    }
}
