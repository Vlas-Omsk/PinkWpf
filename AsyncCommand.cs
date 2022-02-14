using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PinkWpf
{
    public class AsyncCommand : NotifyPropertyChanged, ICommand
    {
        public NotifyTaskCompletion Execution { get; } = new NotifyTaskCompletion();

        private Func<object, Task> _execute;
        private bool _canExecute = true;

        public AsyncCommand(Func<Task> execute) : this(o => execute())
        {
        }

        public AsyncCommand(Func<object, Task> execute)
        {
            _execute = execute;
        }

        #region IsExecuting
        public bool IsExecuting
        {
            get => !_canExecute;
            set
            {
                _canExecute = !value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            await Execution.WatchTaskAsync(_execute?.Invoke(parameter));
            IsExecuting = false;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
