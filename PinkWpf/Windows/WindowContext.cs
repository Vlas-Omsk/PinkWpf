using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace PinkWpf.Windows
{
    public sealed class WindowContext
    {
        public Window Window { get; }
        public IntPtr Hwnd { get; private set; }
        public HwndSource HwndSource { get; private set; }
        public bool IsInstalled { get; private set; }

        private List<IWindowModule> _modules = new List<IWindowModule>();

        internal WindowContext(Window window)
        {
            Window = window;

            Install();
        }

        ~WindowContext()
        {
            Dispose();
        }

        public IReadOnlyCollection<IWindowModule> Modules => _modules.AsReadOnly();

        private void Install()
        {
            Window.Closed += (s, e) => Dispose();
            var windowInteropHelper = new WindowInteropHelper(Window);
            Hwnd = windowInteropHelper.Handle;
            if (Hwnd == IntPtr.Zero)
            {
                DeferredInstall();
                return;
            }
            HwndSource = HwndSource.FromHwnd(Hwnd);
            if (HwndSource == null)
            {
                DeferredInstall();
                return;
            }
            InstallInternal();
        }

        private void DeferredInstall()
        {
            RoutedEventHandler handler = null;
            handler = (sender, e) =>
            {
                var windowInteropHelper = new WindowInteropHelper(Window);
                Hwnd = windowInteropHelper.Handle;
                HwndSource = HwndSource.FromHwnd(Hwnd);
                InstallInternal();
                Window.Loaded -= handler;
            };
            Window.Loaded += handler;
        }

        private void InstallInternal()
        {
            IsInstalled = true;

            for (var i = 0; i < _modules.Count; i++)
            {
                var module = _modules[i];
                InstallModuleInternal(module);
            }
        }

        public T InstallModule<T>() where T : IWindowModule, new()
        {
            var module = _modules.FirstOrDefault(x => x is T);
            if (module == null)
            {
                module = new T();
                _modules.Add(module);

                if (IsInstalled)
                    InstallModuleInternal(module);
            }

            return (T)module;
        }

        private void InstallModuleInternal(IWindowModule module)
        {
            module.Install(this);
            HwndSource.AddHook(module.Hook);
        }

        public void UninstallModule(IWindowModule module)
        {
            if (!_modules.Remove(module))
                return;

            HwndSource.RemoveHook(module.Hook);
            module.Dispose();
        }

        public void Dispose()
        {
            for (var i = 0; i < _modules.Count; i++)
            {
                var module = _modules[0];
                UninstallModule(module);
            }

            IsInstalled = false;
        }
    }
}
