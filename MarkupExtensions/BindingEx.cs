using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PinkWpf.MarkupExtensions
{
    [ContentProperty(nameof(Binding))]
    public class BindingEx : MarkupExtension
    {
        public BindingBase Binding { get; set; }
        public PropertyPath Path { get; set; }
        public BindingMode Mode { get; set; }
        public IValueConverter Converter { get; set; }
        public BindingBase ConverterParameter { get; set; }
        public object TargetNullValue { get; set; }

        public BindingEx()
        {
        }

        public BindingEx(string path)
        {
            Binding = new Binding(path);
        }

        public BindingEx(Binding binding)
        {
            Binding = binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding();
            SetMode(Binding, Mode);
            multiBinding.Bindings.Add(Binding);
            if (ConverterParameter != null)
            {
                SetMode(ConverterParameter, BindingMode.OneWay);
                multiBinding.Bindings.Add(ConverterParameter);
            }
            var adapter = new MultiValueConverterAdapter { Converter = Converter };
            multiBinding.Converter = adapter;
            multiBinding.ConverterParameter = this;
            return multiBinding.ProvideValue(serviceProvider);
        }

        private static void SetMode(BindingBase binding, BindingMode mode)
        {
            if (binding is Binding)
                ((Binding)binding).Mode = mode;
            else if (binding is MultiBinding)
                ((MultiBinding)binding).Mode = mode;
            else
                throw new Exception();
        }

        [ContentProperty(nameof(Converter))]
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            public IValueConverter Converter { get; set; }

            private object lastParameter;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (Converter == null) return values[0]; // Required for VS design-time
                if (values.Length > 1) lastParameter = values[1];

                var bindingEx = (BindingEx)parameter;
                var obj = Converter.Convert(values[0], targetType, lastParameter, culture);

                if (obj == null)
                    return bindingEx.TargetNullValue;

                if (bindingEx.Path != null)
                {
                    var binding = new Binding();
                    binding.Source = obj;
                    binding.Path = bindingEx.Path;
                    binding.Mode = BindingMode.OneTime;
                    binding.TargetNullValue = bindingEx.TargetNullValue;

                    var evaluator = new BindingEvaluator();
                    BindingOperations.SetBinding(evaluator, BindingEvaluator.TargetProperty, binding);

                    return evaluator.Target;
                }
                else
                {
                    return obj;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (Converter == null) return new object[] { value }; // Required for VS design-time

                return new object[] { Converter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
            }
        }

        private class BindingEvaluator : DependencyObject
        {
            public object Target
            {
                get => GetValue(TargetProperty);
                set => SetValue(TargetProperty, value);
            }

            public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(object), typeof(BindingEvaluator));
        }
    }
}
