using System;
using System.Windows;

namespace PinkWpf.Controls
{
    public class BindingEvaluator : FrameworkElement
    {
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingEvaluator));
    }
}
