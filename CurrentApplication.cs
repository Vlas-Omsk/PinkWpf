using System;
using System.ComponentModel;
using System.Windows;

namespace PinkWpf
{
    public class CurrentApplication<TCurrentApplication, TWindow, TWindowViewModel> : NotifyPropertyChanged
        where TCurrentApplication : CurrentApplication<TCurrentApplication, TWindow, TWindowViewModel>, new()
        where TWindow : Window
        where TWindowViewModel : INotifyPropertyChanged
    {
        public static TCurrentApplication Instance { get; } = new TCurrentApplication();

        protected TWindow MainWindow => (TWindow)Application.Current.MainWindow;
        protected TWindowViewModel MainWindowViewModel => (TWindowViewModel)MainWindow.DataContext;

        protected CurrentApplication()
        {
        }
    }
}
