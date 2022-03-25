using PinkWpf.WinApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace PinkWpf.Windows
{
    public sealed class WindowHelper : IDisposable
    {
        public Window Window { get; }
        public IntPtr Hwnd { get; private set; }

        private HwndSource _hwndSource;
        private List<WindowModule> _modules = new List<WindowModule>();
        private bool _isInstalled;

        public WindowHelper(Window window)
        {
            Window = window;

            Install();
        }

        ~WindowHelper()
        {
            Dispose();
        }

        public IReadOnlyCollection<WindowModule> Modules => _modules.AsReadOnly();

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
            _hwndSource = HwndSource.FromHwnd(Hwnd);
            if (_hwndSource == null)
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
                _hwndSource = HwndSource.FromHwnd(Hwnd);
                InstallInternal();
                Window.Loaded -= handler;
            };
            Window.Loaded += handler;
        }

        private void InstallInternal()
        {
            _isInstalled = true;

            foreach (var module in _modules)
                InstallModuleInternal(module);
        }

        public void InstallModule(WindowModule module)
        {
            _modules.Add(module);

            if (_isInstalled)
                InstallModuleInternal(module);
        }

        private void InstallModuleInternal(WindowModule module)
        {
            module.Install(this);
            _hwndSource.AddHook(module.Hook);
        }

        public void UninstallModule(WindowModule module)
        {
            if (!_modules.Remove(module))
                return;

            _hwndSource.RemoveHook(module.Hook);
            module.Dispose();
        }

        public void Dispose()
        {
            foreach (var module in _modules.ToArray())
                UninstallModule(module);

            _isInstalled = false;
        }

        public void HideWindowSysMenu()
        {
            User32.SetWindowLong(Hwnd, GWL.STYLE, User32.GetWindowLong(Hwnd, GWL.STYLE) & ~(long)WS.SYSMENU);
        }
    }
}
