using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace PinkWpf
{
    public abstract class ViewModel : INavigationListener
    {
        public NotifyTaskCompletion Initialization { get; } = new NotifyTaskCompletion();
        public NotifyTaskCompletion Navigation { get; } = new NotifyTaskCompletion();

        public ViewModel()
        {
#if DEBUG
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
#endif
                Initialization.Run(OnInitializationAsync());
        }

        #region INavigationListener

        #region Navigate
        Task<bool> INavigationListener.Navigating()
        {
            return Navigation.RunAsync(OnNavigatingAsync());
        }

        /// <inheritdoc cref="INavigationListener.Navigating"/>
        protected virtual Task<bool> OnNavigatingAsync()
        {
            return Task.FromResult(true);
        }

        Task INavigationListener.Navigated()
        {
            return Navigation.RunAsync(OnNavigatedAsync());
        }

        /// <inheritdoc cref="INavigationListener.Navigated"/>
        protected virtual Task OnNavigatedAsync()
        {
            return Task.CompletedTask;
        }
        #endregion

        #region NavigateTo
        Task<bool> INavigationListener.NavigatingTo()
        {
            return Navigation.RunAsync(OnNavigatingToAsync());
        }

        /// <inheritdoc cref="INavigationListener.NavigatingTo"/>
        protected virtual Task<bool> OnNavigatingToAsync()
        {
            return Task.FromResult(true);
        }

        Task INavigationListener.NavigatedTo()
        {
            return Navigation.RunAsync(OnNavigatedToAsync());
        }

        /// <inheritdoc cref="INavigationListener.NavigatedTo"/>
        protected virtual Task OnNavigatedToAsync()
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Destroy
        Task<bool> INavigationListener.Destroying()
        {
            return Navigation.RunAsync(OnDestroyingAsync());
        }

        /// <inheritdoc cref="INavigationListener.Destroying"/>
        protected virtual Task<bool> OnDestroyingAsync()
        {
            return Task.FromResult(true);
        }

        Task INavigationListener.Destroyed()
        {
            return Navigation.RunAsync(OnDestroyedAsync());
        }

        /// <inheritdoc cref="INavigationListener.Destroyed"/>
        protected virtual Task OnDestroyedAsync()
        {
            return Task.CompletedTask;
        }
        #endregion

        #endregion

        /// <summary>
        /// Происходит при создании элемента
        /// </summary>
        protected virtual Task OnInitializationAsync()
        {
            return Task.CompletedTask;
        }
    }
}
