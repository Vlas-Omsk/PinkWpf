using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PinkWpf.MarkupExtensions.Converters
{
    [ValueConversion(typeof(object), typeof(object), ParameterType = typeof(PropertyPath))]
    public class PathConverter : ValueConverter
    {
        protected override object ConvertInternal(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PropertyPath propertyPath;
            if (parameter is PropertyPath)
                propertyPath = (PropertyPath)parameter;
            else
                propertyPath = new PropertyPath(parameter.ToString());

            var binding = new Binding();
            binding.Source = value;
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
