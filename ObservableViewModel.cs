using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PinkWpf
{
    public abstract class ObservableViewModel : ViewModel, INotifyPropertyChanged, INotifyPropertyChanging
    {
        protected PropertyChangedEventHandler BindProperty(INotifyPropertyChanged notifyPropertyChanged, string sourceName, string targetName)
        {
            void handler(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == sourceName)
                    RaisePropertyChanged(targetName);
            }
            notifyPropertyChanged.PropertyChanged += handler;
            return handler;
        }

        protected void UnbindProperty(INotifyPropertyChanged notifyPropertyChanged, PropertyChangedEventHandler handler)
        {
            notifyPropertyChanged.PropertyChanged -= handler;
        }

        #region ObservableObject
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
        #endregion
    }
}
