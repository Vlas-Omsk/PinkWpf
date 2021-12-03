using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PinkWpf
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
