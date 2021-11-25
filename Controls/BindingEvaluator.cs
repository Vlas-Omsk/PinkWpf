using System;
using System.Windows;

namespace PinkWpf.Controls
{
    public class BindingEvaluator : FrameworkElement
    {
        public object Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(object), typeof(BindingEvaluator));
    }
}
