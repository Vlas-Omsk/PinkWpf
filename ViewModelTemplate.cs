using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace PinkWpf
{
    public abstract class ViewModel<T> : NotifyPropertyChanged where T : FrameworkElement
    {
        public Command OpenLinkCommand { get; private set; }

        public T View { get; private set; }

        private List<(string, string)> _registeredObjects = new List<(string, string)>();

        public ViewModel()
        {
            OpenLinkCommand = new Command(e => OpenLink(e?.ToString()));
        }

        public object GetProperty(string name)
        {
            var parent = (FrameworkElement)View?.Parent;
            if (parent == null)
                return null;
            return parent.Dispatcher.Invoke(() =>
            {
                var property = GetProperty(name, parent);
                return property.GetValue(parent.DataContext);
            });
        }

        public void SetProperty(string name, object value)
        {
            var parent = (FrameworkElement)View?.Parent;
            if (parent == null)
                return;
            parent.Dispatcher.Invoke(() =>
            {
                var property = GetProperty(name, parent);
                property.SetValue(parent.DataContext, value);
            });
        }

        public void BindProperty(string sourceName, string targetName)
        {
            var parent = (FrameworkElement)View?.Parent;
            if (parent == null)
            {
                _registeredObjects.Add((sourceName, targetName));
                return;
            }
            if (parent.DataContext is NotifyPropertyChanged)
            {
                var notifyPropertyChanged = (NotifyPropertyChanged)parent.DataContext;
                notifyPropertyChanged.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == sourceName)
                        OnPropertyChanged(targetName);
                };
                OnPropertyChanged(targetName);
            }
            else
                throw new Exception("DataContext is not NotifyPropertyChanged");
        }

        public void BindGlobalProperty(INotifyPropertyChanged notifyPropertyChanged, string sourceName, string targetName)
        {
            notifyPropertyChanged.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == sourceName)
                    OnPropertyChanged(targetName);
            };
        }

        private PropertyInfo GetProperty(string name, FrameworkElement parent)
        {
            var parentDataContext = parent.DataContext;
            var property = parentDataContext.GetType().GetProperty(name);
            if (property == null)
                throw new Exception("Object not found");
            return property;
        }

        private void OnViewLoadedInternal(object sender, RoutedEventArgs e)
        {
            if (_registeredObjects.Count > 0)
            {
                foreach (var registeredObject in _registeredObjects)
                    BindProperty(registeredObject.Item1, registeredObject.Item2);
                _registeredObjects.Clear();
            }
            OnViewLoaded(sender, e);
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
                if (e.NewValue is ViewModel<T>)
                {
                    var dataContext = (ViewModel<T>)e.NewValue;
                    dataContext.View = (T)sender;
                    if (dataContext.View.IsLoaded)
                        dataContext.OnViewLoadedInternal(sender, new RoutedEventArgs(FrameworkElement.LoadedEvent));
                    else
                        dataContext.View.Loaded += dataContext.OnViewLoadedInternal;
                }
            };
        }
    }
}
