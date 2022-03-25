using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PinkWpf
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        protected void SetValue<T>(out T field, T value, [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName);
            field = value;
            RaisePropertyChanged(propertyName);
        }

        protected void SetValue(Action action, [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName);
            action?.Invoke();
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanging([CallerMemberName] string propertyName = "")
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
