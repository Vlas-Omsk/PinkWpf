using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PinkWpf.MarkupExtensions.AttachedProperties
{
    public sealed class DesignMode : DependencyObject
    {
        #region StyleProperty

        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
           nameof(StyleProperty), typeof(Style), typeof(DesignMode),
           new FrameworkPropertyMetadata(null, OnStyleChanged));

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d) && d is Control control && e.NewValue is Style style)
                control.Style = style;
        }

        public static Style GetStyle(DependencyObject dependencyObject)
        {
            return (Style)dependencyObject.GetValue(StyleProperty);
        }

        public static void SetStyle(DependencyObject dependencyObject, Style value)
        {
            dependencyObject.SetValue(StyleProperty, value);
        }

        #endregion
    }
}
