using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PinkWpf.Controls
{
    public class ExtendedDataGrid : DataGrid
    {
        private bool _rowNumbersDisplayed;

        public ExtendedDataGrid()
        {
            ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
        }

        private void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                UpdateRowNumbers();
            }
        }

        private void UpdateRowNumbers()
        {
            if (DisplayRowNumber)
                _rowNumbersDisplayed = true;

            if (!_rowNumbersDisplayed)
                return;

            if (!DisplayRowNumber)
                _rowNumbersDisplayed = false;

            for (var i = 0; i < ItemContainerGenerator.Items.Count; i++)
            {
                var container = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(i);

                if (container == null)
                    continue;

                if (_rowNumbersDisplayed)
                    container.Header = (i + 1).ToString();
                else
                    container.Header = null;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                HandleSelect(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                HandleSelect(e);
        }

        private void HandleSelect(RoutedEventArgs e)
        {
            if (SelectCommand == null)
                return;

            e.Handled = true;

            SelectCommand?.Execute(null);
        }

        protected override void OnCanExecuteDelete(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DeleteCommand == null;
            e.Handled = true;

            DeleteCommand?.Execute(null);
        }

        #region DeleteCommandProperty

        public new ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public readonly static DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(ExtendedDataGrid),
            new PropertyMetadata()
        );

        #endregion

        #region SelectCommandProperty

        public ICommand SelectCommand
        {
            get => (ICommand)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        public readonly static DependencyProperty SelectCommandProperty = DependencyProperty.Register(
            nameof(SelectCommand),
            typeof(ICommand),
            typeof(ExtendedDataGrid),
            new PropertyMetadata()
        );

        #endregion

        #region DisplayRowNumberProperty

        public bool DisplayRowNumber
        {
            get => (bool)GetValue(DisplayRowNumberProperty);
            set => SetValue(DisplayRowNumberProperty, value);
        }

        public readonly static DependencyProperty DisplayRowNumberProperty = DependencyProperty.Register(
            nameof(DisplayRowNumber),
            typeof(bool),
            typeof(ExtendedDataGrid),
            new PropertyMetadata(false, OnDisplayRowNumberChanged)
        );

        private static void OnDisplayRowNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (ExtendedDataGrid)d;

            dataGrid.UpdateRowNumbers();
        }

        #endregion
    }
}
