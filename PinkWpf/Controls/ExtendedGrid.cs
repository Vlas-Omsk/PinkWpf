using PinkWpf.GridDefinitionsMarkup;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PinkWpf.Controls
{
    public class ExtendedGrid : Grid
    {
        private const string _defaultRowResourceKey = "DefaultRowGap";
        private const string _defaultColumnResourceKey = "DefaultColumnGap";

        public ExtendedGrid()
        {
            ResourceLocator = new FrameworkElementResourceLocatorAccessor(this);
        }

        private IResourceLocator ResourceLocator { get; }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (AutoPositioning)
            {
                UpdateChildrenPositions();
            }
            else
            {
                var absoluteFirstColumn = GetAbsoluteColumn(0);
                SetColumn((UIElement)visualAdded, absoluteFirstColumn);

                var absoluteFirstRow = GetAbsoluteRow(0);
                SetRow((UIElement)visualAdded, absoluteFirstRow);
            }
        }

        private void UpdatePosition(UIElement element)
        {
            var row = GetRelativeRow(element);
            var column = GetRelativeColumn(element);

            SetElementPosition(element, column, row);
        }

        private void UpdateAutoPositions()
        {
            var maxColumns = ColumnDefinitions.Count(x => !GetIsGap(x));

            if (maxColumns == 0)
                maxColumns = 1;

            var column = 0;
            var row = 0;

            foreach (UIElement child in Children)
            {
                var childRow = GetRelativeRow(child);
                var childColumn = GetRelativeColumn(child);

                if (childRow != -1)
                {
                    row = childRow;
                    column = 0;
                }

                if (childColumn != -1)
                {
                    column = childColumn;
                }

                if (column >= maxColumns)
                {
                    row++;
                    column = 0;
                }

                SetElementPosition(child, column, row);

                column++;
            }
        }

        private void SetElementPosition(UIElement element, int relativeColumn, int relativeRow)
        {
            var absoluteColumn = GetAbsoluteColumn(relativeColumn);
            SetColumn(element, absoluteColumn);
            var absoluteColumnSpan = GetAbsoluteColumnSpan(relativeColumn, GetRelativeColumnSpan(element));
            SetColumnSpan(element, absoluteColumnSpan);

            var absoluteRow = GetAbsoluteRow(relativeRow);
            SetRow(element, absoluteRow);
            var absoluteRowSpan = GetAbsoluteRowSpan(relativeRow, GetRelativeRowSpan(element));
            SetRowSpan(element, absoluteRowSpan);
        }

        private void UpdateChildrenPositions()
        {
            if (AutoPositioning)
            {
                UpdateAutoPositions();
            }
            else
            {
                foreach (UIElement children in Children)
                    UpdatePosition(children);
            }
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

        private static int GetAbsolute(int relative, IEnumerable<DefinitionBase> definitions)
        {
            if (relative == -1)
                relative = 0;

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
            nameof(IsGapProperty),
            typeof(bool),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata()
        );

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
            nameof(RelativeColumnProperty),
            typeof(int),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(-1, OnRelativeColumnChanged)
        );

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
            var grid = (ExtendedGrid)((FrameworkElement)d).Parent;
            grid.UpdateChildrenPositions();
        }
        #endregion

        #region RelativeRowProperty

        public static readonly DependencyProperty RelativeRowProperty = DependencyProperty.RegisterAttached(
            nameof(RelativeRowProperty),
            typeof(int),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(-1, OnRelativeRowChanged)
        );

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
            var grid = (ExtendedGrid)((FrameworkElement)d).Parent;
            grid.UpdateChildrenPositions();
        }

        #endregion

        #region RelativeColumnSpanProperty

        public static readonly DependencyProperty RelativeColumnSpanProperty = DependencyProperty.RegisterAttached(
            nameof(RelativeColumnSpanProperty),
            typeof(int),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(1, OnRelativeColumnSpanChanged)
        );

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
            var grid = (ExtendedGrid)((FrameworkElement)d).Parent;
            grid.UpdateChildrenPositions();
        }

        #endregion

        #region RelativeRowSpanProperty

        public static readonly DependencyProperty RelativeRowSpanProperty = DependencyProperty.RegisterAttached(
            nameof(RelativeRowSpanProperty),
            typeof(int),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(1, OnRelativeRowSpanChanged)
        );

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
            var grid = (ExtendedGrid)((FrameworkElement)d).Parent;
            grid.UpdateChildrenPositions();
        }

        #endregion

        #region ColumnsProperty

        public string Columns
        {
            get => (string)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            nameof(Columns),
            typeof(string),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(OnColumnsChanged)
        );

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (ExtendedGrid)d;
            var newValue = (string)e.NewValue;
            var gridDefinitionsParser = new GridDefinitionsMarkupParser(grid.ResourceLocator, _defaultColumnResourceKey);
            var definitions = gridDefinitionsParser.ParseDefinitions(newValue);

            var i = 0;
            foreach (var definition in definitions)
            {
                ColumnDefinition columnDefinition;

                if (i < grid.ColumnDefinitions.Count)
                {
                    columnDefinition = grid.ColumnDefinitions[i];
                }
                else
                {
                    columnDefinition = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(columnDefinition);
                }

                if (definition.Size.HasValue)
                    columnDefinition.Width = definition.Size.Value;
                else
                    columnDefinition.Width = (GridLength)ColumnDefinition.WidthProperty.DefaultMetadata.DefaultValue;

                if (definition.MinSize.HasValue)
                    columnDefinition.MinWidth = definition.MinSize.Value;
                else
                    columnDefinition.MinWidth = (double)ColumnDefinition.MinWidthProperty.DefaultMetadata.DefaultValue;

                if (definition.MaxSize.HasValue)
                    columnDefinition.MaxWidth = definition.MaxSize.Value;
                else
                    columnDefinition.MaxWidth = (double)ColumnDefinition.MaxWidthProperty.DefaultMetadata.DefaultValue;

                SetIsGap(columnDefinition, definition.IsGap);

                i++;
            }

            if (grid.ColumnDefinitions.Count > i)
                grid.ColumnDefinitions.RemoveRange(i, grid.ColumnDefinitions.Count - i);

            grid.UpdateChildrenPositions();
        }

        #endregion

        #region RowsProperty

        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
            nameof(Rows),
            typeof(string),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(OnRowsChanged)
        );

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (ExtendedGrid)d;
            var newValue = (string)e.NewValue;
            var gridDefinitionsParser = new GridDefinitionsMarkupParser(grid.ResourceLocator, _defaultRowResourceKey);
            var definitions = gridDefinitionsParser.ParseDefinitions(newValue);

            var i = 0;
            foreach (var definition in definitions)
            {
                RowDefinition rowDefinition;

                if (i < grid.RowDefinitions.Count)
                {
                    rowDefinition = grid.RowDefinitions[i];
                }
                else
                {
                    rowDefinition = new RowDefinition();
                    grid.RowDefinitions.Add(rowDefinition);
                }

                if (definition.Size.HasValue)
                    rowDefinition.Height = definition.Size.Value;
                else
                    rowDefinition.Height = (GridLength)RowDefinition.HeightProperty.DefaultMetadata.DefaultValue;

                if (definition.MinSize.HasValue)
                    rowDefinition.MinHeight = definition.MinSize.Value;
                else
                    rowDefinition.MinHeight = (double)RowDefinition.MinHeightProperty.DefaultMetadata.DefaultValue;

                if (definition.MaxSize.HasValue)
                    rowDefinition.MaxHeight = definition.MaxSize.Value;
                else
                    rowDefinition.MaxHeight = (double)RowDefinition.MaxHeightProperty.DefaultMetadata.DefaultValue;

                SetIsGap(rowDefinition, definition.IsGap);

                i++;
            }

            if (grid.RowDefinitions.Count > i)
                grid.RowDefinitions.RemoveRange(i, grid.RowDefinitions.Count - i);

            grid.UpdateChildrenPositions();
        }

        #endregion

        #region AutoPositioningProperty

        public bool AutoPositioning
        {
            get => (bool)GetValue(AutoPositioningProperty);
            set => SetValue(AutoPositioningProperty, value);
        }

        public static readonly DependencyProperty AutoPositioningProperty = DependencyProperty.RegisterAttached(
            nameof(AutoPositioning),
            typeof(bool),
            typeof(ExtendedGrid),
            new FrameworkPropertyMetadata(OnAutoPositioningChanged)
        );

        private static void OnAutoPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (ExtendedGrid)d;
            grid.UpdateChildrenPositions();
        }

        #endregion
    }
}
