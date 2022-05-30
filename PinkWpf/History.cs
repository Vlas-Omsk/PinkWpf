using PinkWpf.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace PinkWpf
{
    public enum HistoryMode
    {
        NoSave,
        SavePreviousOnly,
        SaveAll
    }

    public sealed class History : ObservableObject
    {
        public AsyncCommand PushCommand { get; }
        public AsyncCommand NextCommand { get; }
        public AsyncCommand BackCommand { get; }
        public AsyncCommand ReplaceCommand { get; }

        private readonly List<FrameworkElement> _history = new List<FrameworkElement>();
        private int _position = -1;
        private readonly bool _saveNext;
        private readonly bool _savePrevious;

        public History(HistoryMode mode)
        {
            if (mode == HistoryMode.SaveAll)
            {
                _savePrevious = true;
                _saveNext = true;
            }
            if (mode.HasFlag(HistoryMode.SavePreviousOnly))
                _savePrevious = true;

            PushCommand = new AsyncCommand((parameter) =>
            {
                if (!(parameter is FrameworkElement element))
                    throw new ArgumentException("parameter is not FrameworkElement");
                return Push(element);
            });
            NextCommand = new AsyncCommand(Next);
            BackCommand = new AsyncCommand(Back);
            ReplaceCommand = new AsyncCommand((parameter) =>
            {
                if (!(parameter is FrameworkElement element))
                    throw new ArgumentException("parameter is not FrameworkElement");
                return Replace(element);
            });
        }

        public FrameworkElement CurrentElement
        {
            get => _position == -1 ? null : _history[_position];
            set => Replace(value).GetAwaiter().GetResult();
        }

        public FrameworkElement NextElement
        {
            get => CanNavigateNext ? _history[_position + 1] : null;
            set => ReplaceAt(_position + 1, value).GetAwaiter().GetResult();
        }

        public FrameworkElement PreviousElement
        {
            get => CanNavigateBack ? _history[_position - 1] : null;
            set => ReplaceAt(_position - 1, value).GetAwaiter().GetResult();
        }

        public bool CanNavigateNext => _saveNext && _position < _history.Count - 1;
        public bool CanNavigateBack => _savePrevious && _position > 0;

        public IReadOnlyCollection<FrameworkElement> Elements => _history.AsReadOnly();

        public int Position => _position;

        public async Task<bool> Push(FrameworkElement element)
        {
            var navigationListener = element.DataContext as INavigationListener;
            var previousElement = CurrentElement;
            var previousNavigationListener = previousElement?.DataContext as INavigationListener;

            if ((previousNavigationListener == null || await (_savePrevious ? previousNavigationListener.NavigatingTo() : previousNavigationListener.Destroying())) &&
                (navigationListener == null || await navigationListener.Navigating()))
            {
                await RunWithNotify(async () =>
                {
                    await ClearElements(true);
                    _position++;
                    _history.Add(element);
                });
                if (previousNavigationListener != null)
                    await (_savePrevious ? previousNavigationListener.NavigatedTo() : previousNavigationListener.Destroyed());
                if (navigationListener != null)
                    await navigationListener.Navigated();
                return true;
            }
            return false;
        }

        public async Task<bool> Next()
        {
            if (!CanNavigateNext)
                throw new Exception("Can't navigate next");

            var element = CurrentElement;
            var navigationListener = element.DataContext as INavigationListener;
            var nextElement = NextElement;
            var nextNavigationListener = nextElement.DataContext as INavigationListener;

            if ((navigationListener == null || await (_savePrevious ? navigationListener.NavigatingTo() : navigationListener.Destroying())) &&
                (nextNavigationListener == null || await nextNavigationListener.Navigating()))
            {
                await RunWithNotify(async () =>
                {
                    _position++;
                    await ClearElements();
                });
                if (navigationListener != null)
                    await (_savePrevious ? navigationListener.NavigatedTo() : navigationListener.Destroyed());
                if (nextNavigationListener != null)
                    await nextNavigationListener.Navigated();
                return true;
            }
            return false;
        }

        public async Task<bool> Back()
        {
            if (!CanNavigateBack)
                throw new Exception("Can't navigate back");

            var element = CurrentElement;
            var navigationListener = element.DataContext as INavigationListener;
            var previousElement = PreviousElement;
            var previousNavigationListener = previousElement.DataContext as INavigationListener;

            if ((navigationListener == null || await (_saveNext ? navigationListener.NavigatingTo() : navigationListener.Destroying())) &&
                (previousNavigationListener == null || await previousNavigationListener.Navigating()))
            {
                await RunWithNotify(async () =>
                {
                    _position--;
                    await ClearElements();
                });
                if (navigationListener != null)
                    await (_saveNext ? navigationListener.NavigatedTo() : navigationListener.Destroyed());
                if (previousNavigationListener != null)
                    await previousNavigationListener.Navigated();
                return true;
            }
            return false;
        }

        public async Task<bool> Replace(FrameworkElement element)
        {
            if (_position == -1)
                return await Push(element);

            var navigationListener = element.DataContext as INavigationListener;
            var previousElement = CurrentElement;
            var previousNavigationListener = previousElement.DataContext as INavigationListener;

            if ((previousNavigationListener == null || await previousNavigationListener.Destroying()) &&
                (navigationListener == null || await navigationListener.Navigating()))
            {
                await RunWithNotify(() =>
                {
                    _history[_position] = element;
                    return Task.CompletedTask;
                });
                if (previousNavigationListener != null)
                    await previousNavigationListener.Destroyed();
                if (navigationListener != null)
                    await navigationListener.Navigated();
                return true;
            }
            return false;
        }

        public async Task<bool> ReplaceAt(int position, FrameworkElement element)
        {
            if (_position == position)
                return await Replace(element);

            if (position < 0 || position >= _history.Count)
                throw new IndexOutOfRangeException();

            var previousElement = _history[position];
            var previousNavigationListener = previousElement.DataContext as INavigationListener;

            if (previousNavigationListener == null || await previousNavigationListener.Destroying())
            {
                _history[position] = element;
                if (previousNavigationListener != null)
                    await previousNavigationListener.Destroyed();
                return true;
            }
            return false;
        }

        private async Task RunWithNotify(Func<Task> task)
        {
            RaisePropertyChanging(nameof(CurrentElement));
            RaisePropertyChanging(nameof(NextElement));
            RaisePropertyChanging(nameof(PreviousElement));
            RaisePropertyChanging(nameof(CanNavigateBack));
            RaisePropertyChanging(nameof(CanNavigateNext));
            RaisePropertyChanging(nameof(Elements));
            RaisePropertyChanging(nameof(Position));
            await task();
            RaisePropertyChanged(nameof(CurrentElement));
            RaisePropertyChanged(nameof(NextElement));
            RaisePropertyChanged(nameof(PreviousElement));
            RaisePropertyChanged(nameof(CanNavigateBack));
            RaisePropertyChanged(nameof(CanNavigateNext));
            RaisePropertyChanged(nameof(Elements));
            RaisePropertyChanged(nameof(Position));
        }

        private async Task ClearElements(bool noSaveNext = false)
        {
            if (noSaveNext || !_saveNext)
                await RunWithNotify(() => RemoveElementsRange(_position + 1, _history.Count - (_position + 1), false));
            if (!_savePrevious)
                await RunWithNotify(async () =>
                {
                    await RemoveElementsRange(0, _position, false);
                    _position = 0;
                });
        }

        private async Task RemoveElementsRange(int index, int count, bool notifyDestroyed)
        {
            for (var i = index; i < index + count; i++)
            {
                var element = _history[index];
                var navigationListener = element.DataContext as INavigationListener;
                _history.RemoveAt(index);
                if (notifyDestroyed)
                    await navigationListener.Destroyed();
            }
        }

        public Task RemoveNextElements()
        {
            return RunWithNotify(() => RemoveElementsRange(_position + 1, _history.Count - (_position + 1), true));
        }

        public Task RemovePreviousElements()
        {
            return RunWithNotify(async () =>
            {
                await RemoveElementsRange(0, _position, true);
                _position = 0;
            });
        }
    }
}
