using PinkWpf.WinApi;
using System;

namespace PinkWpf.Windows
{
    public sealed class SystemMenuModule : IWindowModule
    {
        public SystemMenu SystemMenu { get; private set; }

        void IWindowModule.Install(WindowContext context)
        {
            var systemMenuHandle = User32.GetSystemMenu(context.Hwnd, false);
            SystemMenu = new SystemMenu(systemMenuHandle, false);
        }

        void IWindowModule.Dispose()
        {
        }

        IntPtr IWindowModule.Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return SystemMenu.Hook(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
