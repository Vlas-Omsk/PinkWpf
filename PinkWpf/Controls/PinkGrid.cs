using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PinkWpf.Controls
{
    public class PinkGrid : Grid
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            var absoluteFirstColumn = GetAbsoluteColumn(0);
            SetColumn((UIElement)visualAdded, absoluteFirstColumn);
            var absoluteFirstRow = GetAbsoluteRow(0);
            SetRow((UIElement)visualAdded, absoluteFirstRow);
        }

        private int GetAbsoluteColumnSpan(int relativeColumn, int relativeColumnSpan)
        {
            if (relativeColumnSpan == 1)
                return 1;
            return GetAbsoluteColumn(relativeColumn + relativeColumnSpan - 1) + 1;
        }

        private int GetAbsoluteRowSpan(int relativeRow, int relativeRowSpan)
        {
            if (relativeRowSpan == 1)
                return 1;
            return GetAbsoluteRow(relativeRow + relativeRowSpan - 1) + 1;
        }

        private int GetAbsoluteColumn(int relativeColumn)
        {
            return GetAbsolute(relativeColumn, ColumnDefinitions);
        }

        private int GetAbsoluteRow(int relativeRow)
        {
            return GetAbsolute(relativeRow, RowDefinitions);
        }

        private int GetAbsolute(int relative, IEnumerable<DefinitionBase> definitions)
        {
            var absolute = 0;
            var currentRelative = 0;

            foreach (var definition in definitions)
            {
                if (!GetIsGap(definition))
                    if (currentRelative++ == relative)
                        return absolute;
                absolute++;
            }

            var count = definitions.Count();
            return count == 0 ? 0 : count - 1;
        }

        #region IsGapProperty
        public static readonly DependencyProperty IsGapProperty = DependencyProperty.RegisterAttached(
           nameof(IsGapProperty), typeof(bool), typeof(PinkGrid),
           new FrameworkPropertyMetadata());

        public static bool GetIsGap(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsGapProperty);
        }

        public static void SetIsGap(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsGapProperty, value);
        }
        #endregion

        #region RelativeColumnProperty
        public static readonly DependencyProperty RelativeColumnProperty = DependencyProperty.RegisterAttached(
           nameof(RelativeColumnProperty), typeof(int), typeof(PinkGrid),
           new FrameworkPropertyMetadata(0, OnRelativeColumnChanged));

        public static int GetRelativeColumn(UIElement element)
        {
            return (int)element.GetValue(RelativeColumnProperty);
        }

        public static void SetRelativeColumn(UIElement element, int value)
        {
            element.SetValue(RelativeColumnProperty, value);
        }

        private static void OnRelativeColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)((FrameworkElement)d).Parent;
            var absoluteColumn = pinkGrid.GetAbsoluteColumn((int)e.NewValue);
            SetColumn((UIElement)d, absoluteColumn);
            var absoluteColumnSpan = pinkGrid.GetAbsoluteColumnSpan(GetRelativeColumn((UIElement)d), GetRelativeColumnSpan((UIElement)d));
            SetColumnSpan((UIElement)d, absoluteColumnSpan);
        }
        #endregion

        #region RelativeRowProperty
        public static readonly DependencyProperty RelativeRowProperty = DependencyProperty.RegisterAttached(
           nameof(RelativeRowProperty), typeof(int), typeof(PinkGrid),
           new FrameworkPropertyMetadata(0, OnRelativeRowChanged));

        public static int GetRelativeRow(UIElement element)
        {
            return (int)element.GetValue(RelativeRowProperty);
        }

        public static void SetRelativeRow(UIElement element, int value)
        {
            element.SetValue(RelativeRowProperty, value);
        }

        private static void OnRelativeRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)((FrameworkElement)d).Parent;
            var absoluteRow = pinkGrid.GetAbsoluteRow((int)e.NewValue);
            SetRow((UIElement)d, absoluteRow);
            var absoluteRowSpan = pinkGrid.GetAbsoluteRowSpan(GetRelativeRow((UIElement)d), GetRelativeRowSpan((UIElement)d));
            SetRowSpan((UIElement)d, absoluteRowSpan);
        }
        #endregion

        #region RelativeColumnSpanProperty
        public static readonly DependencyProperty RelativeColumnSpanProperty = DependencyProperty.RegisterAttached(
           nameof(RelativeColumnSpanProperty), typeof(int), typeof(PinkGrid),
           new FrameworkPropertyMetadata(1, OnRelativeColumnSpanChanged));

        public static int GetRelativeColumnSpan(UIElement element)
        {
            return (int)element.GetValue(RelativeColumnSpanProperty);
        }

        public static void SetRelativeColumnSpan(UIElement element, int value)
        {
            element.SetValue(RelativeColumnSpanProperty, value);
        }

        private static void OnRelativeColumnSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)((FrameworkElement)d).Parent;
            var absoluteColumnSpan = pinkGrid.GetAbsoluteColumnSpan(GetRelativeColumn((UIElement)d), (int)e.NewValue);
            SetColumnSpan((UIElement)d, absoluteColumnSpan);
        }
        #endregion

        #region RelativeRowSpanProperty
        public static readonly DependencyProperty RelativeRowSpanProperty = DependencyProperty.RegisterAttached(
           nameof(RelativeRowSpanProperty), typeof(int), typeof(PinkGrid),
           new FrameworkPropertyMetadata(1, OnRelativeRowSpanChanged));

        public static int GetRelativeRowSpan(UIElement element)
        {
            return (int)element.GetValue(RelativeRowSpanProperty);
        }

        public static void SetRelativeRowSpan(UIElement element, int value)
        {
            element.SetValue(RelativeRowSpanProperty, value);
        }

        private static void OnRelativeRowSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)((FrameworkElement)d).Parent;
            var absoluteRowSpan = pinkGrid.GetAbsoluteRowSpan(GetRelativeRow((UIElement)d), (int)e.NewValue);
            SetRowSpan((UIElement)d, absoluteRowSpan);
        }
        #endregion

        #region ColumnsProperty
        public string Columns
        {
            get => (string)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            nameof(Columns), typeof(string), typeof(PinkGrid),
            new FrameworkPropertyMetadata(OnColumnsChanged));

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)d;
            var newValue = (string)e.NewValue;

            if (string.IsNullOrEmpty(newValue))
                return;

            var split = newValue.Split(',');

            for (var i = 0; i < split.Length; i++)
            {
                var column = split[i];
                var isGap = false;

                if (column.FirstOrDefault() == '[' && column.LastOrDefault() == ']')
                {
                    column = column.Substring(1, column.Length - 2);
                    isGap = true;
                }

                ColumnDefinition definition;

                if (i < pinkGrid.ColumnDefinitions.Count)
                {
                    definition = pinkGrid.ColumnDefinitions[i];
                }
                else
                {
                    definition = new ColumnDefinition();
                    pinkGrid.ColumnDefinitions.Add(definition);
                }

                var columnSplit = column.Split('|');

                definition.Width = GetValue(pinkGrid, columnSplit[0], (GridLength)ColumnDefinition.WidthProperty.DefaultMetadata.DefaultValue);
                definition.MinWidth = columnSplit.Length >= 2 ?
                    GetValue(pinkGrid, columnSplit[1], (double)ColumnDefinition.MinWidthProperty.DefaultMetadata.DefaultValue) :
                    (double)ColumnDefinition.MinWidthProperty.DefaultMetadata.DefaultValue;
                definition.MaxWidth = columnSplit.Length >= 3 ?
                    GetValue(pinkGrid, columnSplit[2], (double)ColumnDefinition.MaxWidthProperty.DefaultMetadata.DefaultValue) :
                    (double)ColumnDefinition.MaxWidthProperty.DefaultMetadata.DefaultValue;

                SetIsGap(definition, isGap);
            }
        }
        #endregion

        #region RowsProperty
        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
            nameof(Rows), typeof(string), typeof(PinkGrid),
            new FrameworkPropertyMetadata(OnRowsChanged));

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pinkGrid = (PinkGrid)d;
            var newValue = (string)e.NewValue;

            if (string.IsNullOrEmpty(newValue))
                return;

            var split = newValue.Split(',');

            for (var i = 0; i < split.Length; i++)
            {
                var column = split[i];
                var isGap = false;

                if (column.FirstOrDefault() == '[' && column.LastOrDefault() == ']')
                {
                    column = column.Substring(1, column.Length - 2);
                    isGap = true;
                }

                RowDefinition definition;

                if (i < pinkGrid.RowDefinitions.Count)
                {
                    definition = pinkGrid.RowDefinitions[i];
                }
                else
                {
                    definition = new RowDefinition();
                    pinkGrid.RowDefinitions.Add(definition);
                }

                var columnSplit = column.Split('|');

                definition.Height = GetValue(pinkGrid, columnSplit[0], (GridLength)ColumnDefinition.WidthProperty.DefaultMetadata.DefaultValue);
                definition.MinHeight = columnSplit.Length >= 2 ?
                    GetValue(pinkGrid, columnSplit[1], (double)ColumnDefinition.MinWidthProperty.DefaultMetadata.DefaultValue) :
                    (double)ColumnDefinition.MinWidthProperty.DefaultMetadata.DefaultValue;
                definition.MaxHeight = columnSplit.Length >= 3 ?
                    GetValue(pinkGrid, columnSplit[2], (double)ColumnDefinition.MaxWidthProperty.DefaultMetadata.DefaultValue) :
                    (double)ColumnDefinition.MaxWidthProperty.DefaultMetadata.DefaultValue;

                SetIsGap(definition, isGap);
            }
        }
        #endregion

        private static T GetValue<T>(FrameworkElement owner, string value, T defaultValue)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var isResource = false;

            if (value.FirstOrDefault() == '{' && value.LastOrDefault() == '}')
            {
                value = value.Substring(1, value.Length - 2);
                isResource = true;
            }

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            if (isResource)
                return (T)owner.FindResource(value);
            return (T)converter.ConvertFromString(value);
        }
    }
}
