using PinkWpf.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf.Windows
{
    public sealed class UnborderedWindowModule : IWindowModule
    {
        private int _xborder;
        private int _yborder;
        private Int32Rect _prevRect;
        private WindowContext _context;

        private WindowState WindowState => _context.Window.WindowState;

        void IWindowModule.Install(WindowContext context)
        {
            if (context.Window.WindowStyle == WindowStyle.None)
                throw new InvalidOperationException("WindowStyle.None cannot be used with this module");

            _context = context;

            var styleModule = _context.InstallModule<StyleModule>();
            styleModule.SystemMenuVisible = false;

            _xborder = User32.GetSystemMetrics(SM.CXSIZEFRAME);
            _yborder = User32.GetSystemMetrics(SM.CYSIZEFRAME);
        }

        void IWindowModule.Dispose()
        {
        }

        IntPtr IWindowModule.Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var lRet = IntPtr.Zero;
            var wmMsg = (WM)msg;

            if (wmMsg == WM.NCCALCSIZE && wParam != (IntPtr)0)
            {
                var pars = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);

                pars.rgrc[0].Left = pars.rgrc[0].Left + _xborder;
                pars.rgrc[0].Right = pars.rgrc[0].Right - _xborder * 2 + 1;
                pars.rgrc[0].Bottom = pars.rgrc[0].Bottom - _yborder;

                var monitor = Monitor.FromWindow(_context.Hwnd);

                var rect = _prevRect =
                    WindowState == WindowState.Minimized ?
                    _prevRect :
                    monitor.WorkArea;

                if (pars.rgrc[0].Top < rect.Y)
                    pars.rgrc[0].Top = rect.Y;
                if (pars.rgrc[0].Bottom > rect.Y + rect.Height)
                    pars.rgrc[0].Bottom = rect.Y + rect.Height;

                Marshal.StructureToPtr(pars, lParam, false);

                handled = false;
                return lRet;
            }

            if (wmMsg == WM.NCACTIVATE)
            {
                lRet = (IntPtr)1;
                handled = true;
                return lRet;
            }

            //handled = DwmApi.DwmDefWindowProc(hwnd, msg, wParam, lParam, ref lRet);
            return lRet;
        }
    }
}
