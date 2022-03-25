using System;

namespace PinkWpf.Windows
{
    public abstract class WindowModule
    {
        protected internal abstract void Install(WindowHelper helper);
        protected internal abstract void Dispose();
        protected internal abstract IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    }
}
