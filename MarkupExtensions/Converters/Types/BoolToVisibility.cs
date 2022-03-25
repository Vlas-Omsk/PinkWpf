using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibility : CompositeConverter
    {
        public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

        protected override object ConvertOverride(ConverterArgs e)
        {
            return e.GetSingleValue<bool>() ? Visibility.Visible : InvisibleState;
        }
    }
}
