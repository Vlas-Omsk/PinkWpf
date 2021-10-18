using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        protected TWindowViewModel MainWindowViewModel => ViewModel.GetViewModel<TWindowViewModel>(MainWindow);

        protected CurrentApplication()
        {
        }
    }
}
