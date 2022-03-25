using PinkWpf.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf.Windows
{
    public sealed class UnborderedWindowModule : WindowModule
    {
        private int _xborder;
        private int _yborder;
        private Win32Rect _prevRect;
        private WindowHelper _helper;

        private WindowState WindowState => _helper.Window.WindowState;

        protected internal override void Install(WindowHelper helper)
        {
            _helper = helper;

            _helper.HideWindowSysMenu();

            _xborder = User32.GetSystemMetrics(SM.CXSIZEFRAME);
            _yborder = User32.GetSystemMetrics(SM.CYSIZEFRAME);
        }

        protected internal override void Dispose()
        {
        }

        private MONITORINFO GetCurrentMonitorInfo()
        {
            var monitor = User32.MonitorFromWindow(_helper.Hwnd, MONITOR.DEFAULTTONEAREST);
            MONITORINFO info = new MONITORINFO();
            User32.GetMonitorInfo(monitor, info);
            return info;
        }

        protected internal override IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var lRet = IntPtr.Zero;
            var wmMsg = (WM)msg;

            // Убирает стандартную рамку
            if (wmMsg == WM.NCCALCSIZE && wParam == (IntPtr)0)
            {
                var pars = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);

                pars.rgrc[0].Left = pars.rgrc[0].Left + _xborder;
                pars.rgrc[0].Right = pars.rgrc[0].Right - _xborder * 2 + 1;
                pars.rgrc[0].Bottom = pars.rgrc[0].Bottom - _yborder;

                var rect = _prevRect = WindowState == WindowState.Minimized ? _prevRect : GetCurrentMonitorInfo().rcWork;

                if (pars.rgrc[0].Top < rect.Top)
                    pars.rgrc[0].Top = rect.Top;
                if (pars.rgrc[0].Bottom > rect.Bottom)
                    pars.rgrc[0].Bottom = rect.Bottom;

                Marshal.StructureToPtr(pars, lParam, false);

                handled = true;
                return lRet;
            }

            if (wmMsg == WM.NCACTIVATE)
            {
                lRet = (IntPtr)1;
                handled = true;
                return lRet;
            }

            handled = DwmApi.DwmDefWindowProc(hwnd, msg, wParam, lParam, ref lRet);

            // Обработка нажатий мыши
            if (wmMsg == WM.NCHITTEST && lRet == IntPtr.Zero)
            {
                lRet = HitTestNCA(hwnd, wParam, lParam);

                if (lRet != (IntPtr)HT.NOWHERE)
                    handled = true;
            }

            return lRet;
        }

        private static int GetXLParam(IntPtr lp)
        {
            return (short)((ulong)lp & 0xffff);
        }

        private static int GetYLParam(IntPtr lp)
        {
            return (short)((((ulong)lp) >> 16) & 0xffff);
        }

        // Обработка координат мыши для неклиентской области
        private IntPtr HitTestNCA(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            // Get the point coordinates for the hit test
            var ptMouse = new Point(GetXLParam(lParam), GetYLParam(lParam));

            // Get the window rectangle.
            Win32Rect rcWindow;
            User32.GetWindowRect(hWnd, out rcWindow);

            // Get the frame rectangle, adjusted for the style without a caption.
            Win32Rect rcFrame = new Win32Rect();
            User32.AdjustWindowRectEx(ref rcFrame, WS.OVERLAPPEDWINDOW & ~WS.CAPTION, false, 0);

            // Determine if the hit test is for resizing. Default middle (1,1).
            ushort uRow = 1;
            ushort uCol = 1;
            bool fOnResizeBorder = false;

            // Determine if the point is at the top or bottom of the window.
            if (ptMouse.Y >= rcWindow.Top && ptMouse.Y < rcWindow.Top + _yborder)
            {
                fOnResizeBorder = ptMouse.Y < (rcWindow.Top - rcFrame.Top);
                uRow = 0;
            }
            else if (ptMouse.Y < rcWindow.Bottom && ptMouse.Y >= rcWindow.Bottom - _yborder)
            {
                uRow = 2;
            }

            // Determine if the point is at the left or right of the window.
            if (ptMouse.X >= rcWindow.Left && ptMouse.X < rcWindow.Left + _xborder)
            {
                uCol = 0; // left side
            }
            else if (ptMouse.X < rcWindow.Right && ptMouse.X >= rcWindow.Right - _xborder)
            {
                uCol = 2; // right side
            }

            // Hit test (HTTOPLEFT, ... HTBOTTOMRIGHT)
            IntPtr[,] hitTests = new IntPtr[,]
            {
                { (IntPtr)HT.TOPLEFT, fOnResizeBorder ? (IntPtr)HT.TOP : (IntPtr)HT.CAPTION, (IntPtr)HT.TOPRIGHT },
                { (IntPtr)HT.LEFT,  (IntPtr)HT.NOWHERE, (IntPtr)HT.RIGHT},
                { (IntPtr)HT.BOTTOMLEFT, (IntPtr)HT.BOTTOM, (IntPtr)HT.BOTTOMRIGHT },
            };

            return hitTests[uRow, uCol];
        }
    }
}
