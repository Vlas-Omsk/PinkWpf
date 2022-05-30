using System;

namespace PinkWpf.Windows
{
    public interface IWindowModule
    {
        void Install(WindowContext context);
        void Dispose();
        IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    }
}
