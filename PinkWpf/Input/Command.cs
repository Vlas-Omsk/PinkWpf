using System;
using System.Windows.Input;

namespace PinkWpf.Input
{
    public class Command : ICommand
    {
        private Action<object> _execute;

        public Command(Action execute) : this(o => execute())
        {
        }

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
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
