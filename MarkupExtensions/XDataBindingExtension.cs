using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions
{
    public class XDataBindingExtension : MarkupExtension
    {
        private Binding _binding = new Binding() 
        { 
            Source = XDataGlobal.Instance
        };

        public XDataBindingExtension()
        {
        }

        public XDataBindingExtension(PropertyPath path)
        {
            Path = path;
        }

        public PropertyPath Path
        {
            get => _binding.Path;
            set => _binding.Path = value;
        }

        public BindingMode Mode
        {
            get => _binding.Mode;
            set => _binding.Mode = value;
        }

        public IValueConverter Converter
        {
            get => _binding.Converter;
            set => _binding.Converter = value;
        }

        public CultureInfo ConverterCulture
        {
            get => _binding.ConverterCulture;
            set => _binding.ConverterCulture = value;
        }

        public object ConverterParameter
        {
            get => _binding.ConverterParameter;
            set => _binding.ConverterParameter = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _binding.ProvideValue(serviceProvider);
        }
    }
}
