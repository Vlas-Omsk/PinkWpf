using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PinkWpf
{
    public static class ViewModel
    {
        public static T GetViewModel<T>(FrameworkElement element) where T : INotifyPropertyChanged
        {
            return (T)element.DataContext;
        }
    }
}
