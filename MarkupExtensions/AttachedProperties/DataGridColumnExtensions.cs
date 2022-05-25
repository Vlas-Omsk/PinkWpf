using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PinkWpf.MarkupExtensions.AttachedProperties
{
    public sealed class DataGridColumnExtensions
    {
        #region FocusableProperty

        private static readonly PropertyInfo _dataGridColumnDataGridOwnerProperty = typeof(DataGridColumn).GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Dictionary<DataGrid, DataGridListener> _dataGridListeners = new Dictionary<DataGrid, DataGridListener>();

        private class DataGridListener
        {
            public EventHandler Handler { get; }
            public List<int> ColumnIndexes { get; } = new List<int>();

            public DataGridListener(EventHandler listener)
            {
                Handler = listener;
            }
        }

        public static readonly DependencyProperty FocusableProperty = DependencyProperty.RegisterAttached(
           nameof(FocusableProperty), typeof(bool), typeof(DataGridColumnExtensions),
           new FrameworkPropertyMetadata(true, OnFocusableChanged));

        private static async void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGridColumn))
                throw new Exception("d.GetType() != typeof(DataGridColumn)");

            var dataGridColumn = (DataGridColumn)d;
            DataGrid dataGrid = null;

            await Task.Run(() =>
            {
                while (true)
                {
                    if ((dataGrid = (DataGrid)_dataGridColumnDataGridOwnerProperty.GetValue(dataGridColumn)) != null)
                        return;
                }
            });

            var columnIndex = -1;
            var i = 0;
            foreach (var column in dataGrid.Columns)
            {
                if (column == dataGridColumn)
                {
                    columnIndex = i;
                    break;
                }
                i++;
            }

            if (columnIndex == -1)
                return;

            if (_dataGridListeners.TryGetValue(dataGrid, out DataGridListener listener))
            {
                listener.ColumnIndexes.Add(columnIndex);
                listener.Handler(null, null);
                return;
            }

            listener = new DataGridListener((sender, ee) =>
            {
                if (dataGrid.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    for (i = 0; i < dataGrid.Items.Count; i++)
                    {
                        var container = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                        var presenter = container.GetChildOfType<DataGridCellsPresenter>();

                        foreach (var j in listener.ColumnIndexes)
                        {
                            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(j);

                            cell.Focusable = GetFocusable(dataGridColumn);
                        }
                    }
                }
            });

            listener.ColumnIndexes.Add(columnIndex);

            _dataGridListeners.Add(dataGrid, listener);

            dataGrid.ItemContainerGenerator.StatusChanged += listener.Handler;
            dataGrid.Unloaded += OnDataGridUnloaded;

            listener.Handler(null, null);
        }

        private static void OnDataGridUnloaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var listener = _dataGridListeners[dataGrid];

            dataGrid.ItemContainerGenerator.StatusChanged -= listener.Handler;
            dataGrid.Unloaded -= OnDataGridUnloaded;
        }

        public static bool GetFocusable(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(FocusableProperty);
        }

        public static void SetFocusable(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(FocusableProperty, value);
        }

        #endregion
    }
}
