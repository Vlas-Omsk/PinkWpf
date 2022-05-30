using PinkWpf.WinApi;
using System;

namespace PinkWpf.Windows
{
    public sealed class StyleModule : IWindowModule
    {
        private WindowContext _context;
        private bool _windowSystemMenuVisible;

        public bool SystemMenuVisible
        {
            get => _windowSystemMenuVisible;
            set
            {
                _windowSystemMenuVisible = value;
                UpdateLongs();
            }
        }

        void IWindowModule.Install(WindowContext context)
        {
            _context = context;
            UpdateLongs();
        }

        void IWindowModule.Dispose()
        {
            _context = null;
        }

        IntPtr IWindowModule.Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        private void UpdateLongs()
        {
            if (_context == null)
                return;

            var l = User32.GetWindowLong(_context.Hwnd, GWL.STYLE);

            if (SystemMenuVisible)
                l |= (long)WS.SYSMENU;
            else
                l &= ~(long)WS.SYSMENU;

            User32.SetWindowLong(_context.Hwnd, GWL.STYLE, l);
        }
    }
}
