using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PinkWpf.Input
{
    public class AsyncCommand : ObservableObject, ICommand
    {
        public NotifyTaskCompletion Execution { get; } = new NotifyTaskCompletion();

        private readonly Func<object, Task> _execute;

        public AsyncCommand(Func<object, Task> execute)
        {
            _execute = execute;
        }

        public AsyncCommand(Func<Task> execute) : this(o => execute())
        {
        }

        #region IsExecuting
        private bool _isExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                SetValue(out _isExecuting, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            await Execution.RunAsync(_execute?.Invoke(parameter));
            IsExecuting = false;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
