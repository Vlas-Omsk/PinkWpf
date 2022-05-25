using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions
{
    public class StoreBindingExtension : MarkupExtension
    {
        private string _instanceKey;
        private readonly Binding _binding = new Binding();

        public StoreBindingExtension()
        {
            InstanceKey = StoreGlobal.DefaultInstanceKey;
        }

        public StoreBindingExtension(PropertyPath path) : this()
        {
            Path = path;
        }

        public string InstanceKey
        {
            get => _instanceKey;
            set => _instanceKey = value;
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
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTarget.TargetObject is DependencyObject obj && DesignerProperties.GetIsInDesignMode(obj))
            {
                var dependencyProperty = (DependencyProperty)provideValueTarget.TargetProperty;

                return dependencyProperty.PropertyType.IsValueType ?
                    Activator.CreateInstance(dependencyProperty.PropertyType) :
                    null;
            }

            _binding.Source = StoreGlobal.Instances[_instanceKey];
            return _binding.ProvideValue(serviceProvider);
        }
    }
}
