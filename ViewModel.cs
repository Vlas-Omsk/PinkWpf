using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Reflection;

namespace PinkWpf
{
    public abstract class ViewModel<T> : NotifyPropertyChanged where T : FrameworkElement
    {
        public Command OpenLinkCommand { get; private set; }
        public T View { get; private set; }

        public ViewModel()
        {
            OpenLinkCommand = new Command(e => OpenLink(e?.ToString()));
        }

        public void BindProperty(INotifyPropertyChanged notifyPropertyChanged, string sourceName, string targetName)
        {
            notifyPropertyChanged.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == sourceName)
                    OnPropertyChanged(targetName);
            };
        }

        protected virtual void OnViewLoaded(object sender, RoutedEventArgs e)
        {
        }

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

        public static void BindViewModel(T element)
        {
            element.DataContextChanged += (sender, e) =>
            {
                if (e.NewValue is ViewModel<T> dataContext)
                {
                    dataContext.View = (T)sender;
                    if (dataContext.View.IsLoaded)
                        dataContext.OnViewLoaded(sender, new RoutedEventArgs(FrameworkElement.LoadedEvent));
                    else
                        dataContext.View.Loaded += dataContext.OnViewLoaded;
                }
            };
        }
    }
}
