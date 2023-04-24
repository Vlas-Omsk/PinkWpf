using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PinkWpf.Controls
{
    public sealed class Loader : ContentControl
    {
        private bool _waitForCommandActivation;
        private bool _commandHasExecuted;

        static Loader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loader), new FrameworkPropertyMetadata(typeof(Loader)));
        }

        public Loader()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        ~Loader()
        {
            //Loaded -= OnLoaded;
            //Unloaded -= OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (Command == null)
            {
                _waitForCommandActivation = true;
                return;
            }

            ExecuteCommandIfCommndNotExecuted();
        }

        private void ExecuteCommandIfCommndNotExecuted()
        {
            _waitForCommandActivation = false;

            if (_commandHasExecuted)
                return;

            _commandHasExecuted = true;

            Command.Execute(null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _waitForCommandActivation = false;
        }

        #region StateProperty

        public LoadingState State
        {
            get => (LoadingState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public readonly static DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(LoadingState),
            typeof(Loader),
            new PropertyMetadata(LoadingState.Loading)
        );

        #endregion

        #region StatusProperty

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public readonly static DependencyProperty StatusProperty = DependencyProperty.Register(
            nameof(Status),
            typeof(string),
            typeof(Loader),
            new PropertyMetadata()
        );

        #endregion

        #region LoaderContentProperty

        public object LoaderContent
        {
            get => (object)GetValue(LoaderContentProperty);
            set => SetValue(LoaderContentProperty, value);
        }

        public readonly static DependencyProperty LoaderContentProperty = DependencyProperty.Register(
            nameof(LoaderContent),
            typeof(object),
            typeof(Loader),
            new PropertyMetadata()
        );

        #endregion

        #region CommandProperty

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public readonly static DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(Loader),
            new PropertyMetadata(OnCommandChanged)
        );

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var loader = (Loader)d;

            if (loader._waitForCommandActivation && e.NewValue != null)
                loader.ExecuteCommandIfCommndNotExecuted();
        }

        #endregion
    }
}
