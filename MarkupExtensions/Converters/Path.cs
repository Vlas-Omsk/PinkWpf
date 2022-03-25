using System;
using System.Windows;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters.Specific
{
    [ValueConversion(typeof(object), typeof(object))]
    public class Path : CompositeConverter
    {
        protected override object ConvertOverride(ConverterArgs e)
        {
            if (!(e.Parameter is PropertyPath propertyPath))
                propertyPath = new PropertyPath(e.GetParameter<string>());

            var binding = new Binding();
            binding.Source = e.GetSingleValue();
            binding.Path = propertyPath;
            binding.Mode = BindingMode.OneTime;

            var evaluator = new BindingEvaluator();
            BindingOperations.SetBinding(evaluator, BindingEvaluator.TargetProperty, binding);

            return evaluator.Target;
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
