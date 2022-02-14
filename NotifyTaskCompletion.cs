using System;
using System.Threading.Tasks;

namespace PinkWpf
{
    public sealed class NotifyTaskCompletion : NotifyPropertyChanged
    {
        public Task Task { get; private set; }
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        public async void WatchTask(Task task)
        {
            await WatchTaskAsync(task);
        }

        public async Task WatchTaskAsync(Task task)
        {
            Task = task;

            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsCanceled));
            RaisePropertyChanged(nameof(IsFaulted));
            RaisePropertyChanged(nameof(Exception));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            try
            {
                await task;
            }
            catch
            {
            }

            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(IsCompleted));
            if (task.IsCanceled)
            {
                RaisePropertyChanged(nameof(IsCanceled));
            }
            else if (task.IsFaulted)
            {
                RaisePropertyChanged(nameof(IsFaulted));
                RaisePropertyChanged(nameof(Exception));
            }
            else
            {
                RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            }
        }
    }
}
