using System;
using System.Windows.Input;

namespace PinkWpf
{
    public class Command : ICommand
    {
        private Action<object> _execute;
        private bool _canExecute = true;

        public Command(Action execute) : this(o => execute())
        {
        }

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
