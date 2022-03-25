using System;
using System.Threading.Tasks;

namespace PinkWpf
{
    public sealed class NotifyTaskCompletion : ObservableObject
    {
        public Task Task { get; private set; } = Task.CompletedTask;
        public bool ThrowOnException { get; set; }

        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        public async void Run(Task task)
        {
            await RunAsync(task);
        }

        public async Task<T> RunAsync<T>(Task<T> task)
        {
            await RunAsync((Task)task);
            return task.Result;
        }

        public async Task RunAsync(Task task)
        {
            Task = task;

            RaiseExecution();

            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsCanceled));
            RaisePropertyChanged(nameof(IsFaulted));
            RaisePropertyChanged(nameof(Exception));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            try
            {
                await Task;
            }
            catch
            {
            }

            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(IsCompleted));
            if (Task.IsCanceled)
            {
                RaisePropertyChanged(nameof(IsCanceled));
            }
            else if (Task.IsFaulted)
            {
                RaisePropertyChanged(nameof(IsFaulted));
                RaisePropertyChanged(nameof(Exception));
            }
            else
            {
                RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            }

            RaiseExecuted();

            if (Task.IsFaulted && ThrowOnException)
                throw Exception;
        }

        private void RaiseExecution()
        {
            Execution?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseExecuted()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Execution;
        public event EventHandler Executed;
    }
}
