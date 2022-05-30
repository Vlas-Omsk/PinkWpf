using System;

namespace PinkWpf.MarkupExtensions.Converters
{
    public interface IUniversalConverter
    {
        object Convert(ConverterArgs e);
        object[] ConvertBack(ConverterArgs e);
    }
}
