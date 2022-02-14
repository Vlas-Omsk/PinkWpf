using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace PinkWpf
{
    public abstract class ViewModel<T> : NotifyPropertyChanged where T : FrameworkElement
    {
        public T View { get; private set; }
        public NotifyTaskCompletion Initialization { get; } = new NotifyTaskCompletion();

        public ViewModel()
        {
            Initialization.WatchTask(OnInitializationAsync());
        }

        #region OpenLinkCommand
        public Command OpenLinkCommand { get; } = new Command(e => OpenLink(e?.ToString()));

        public static void OpenLink(string link)
        {
            if (link == null)
                return;

            Process proc = new Process();
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = "/c start " + link;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
        }
        #endregion

        public void BindProperty(INotifyPropertyChanged notifyPropertyChanged, string sourceName, string targetName)
        {
            notifyPropertyChanged.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == sourceName)
                    RaisePropertyChanged(targetName);
            };
        }

        protected virtual Task OnInitializationAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void OnViewLoaded()
        {
        }

        public static void BindViewModel(T element)
        {
            element.DataContextChanged += (sender, e) =>
            {
                if (!(e.NewValue is ViewModel<T> dataContext))
                    return;

                dataContext.View = (T)sender;
                if (dataContext.View.IsLoaded)
                    dataContext.OnViewLoaded();
                else
                    dataContext.View.Loaded += (sender2, e2) => dataContext.OnViewLoaded();
            };
        }
    }
}
