using PinkWpf.NativeStructs;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf
{
    public partial class WindowHelper
    {
        public bool InfinityWindowEnabled { get; private set; }

        private int _xborder;
        private int _yborder;
        private Win32Rect _prevRect;

        public void EnableInfinityWindow()
        {
            if (InfinityWindowEnabled)
                return;
            InfinityWindowEnabled = true;

            HideCloseButton();

            _xborder = GetSystemMetrics(SM.CXSIZEFRAME);
            _yborder = GetSystemMetrics(SM.CYSIZEFRAME);
        }

        private static int GetXLParam(IntPtr lp)
        {
            return (short)((ulong)lp & 0xffff);
        }

        private static int GetYLParam(IntPtr lp)
        {
            return (short)((((ulong)lp) >> 16) & 0xffff);
        }

        //обработчик сообщений для окна
        private IntPtr InfinityWindowWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var lRet = IntPtr.Zero;
            var wmMsg = (WM)msg;

            if (InfinityWindowEnabled)
            {
                if (wmMsg == WM.NCCALCSIZE)
                {
                    if (wParam != (IntPtr)0)
                    {
                        //убираем стандартную рамку сверху
                        lRet = IntPtr.Zero;

                        NCCALCSIZE_PARAMS pars = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));

                        pars.rgrc[0].Left = pars.rgrc[0].Left + _xborder;
                        pars.rgrc[0].Right = pars.rgrc[0].Right - _xborder * 2 + 1;
                        pars.rgrc[0].Bottom = pars.rgrc[0].Bottom - _yborder;

                        var rect = Window.WindowState == WindowState.Minimized ? _prevRect : GetCurrentMonitorInfo().rcWork;
                        _prevRect = rect;

                        if (pars.rgrc[0].Top < rect.Top)
                            pars.rgrc[0].Top = rect.Top;
                        if (pars.rgrc[0].Bottom > rect.Bottom)
                            pars.rgrc[0].Bottom = rect.Bottom;

                        Marshal.StructureToPtr(pars, lParam, false);

                        handled = true;
                        return lRet;
                    }
                }

                if (wmMsg == WM.NCACTIVATE)
                {
                    lRet = (IntPtr)1;
                    handled = true;
                    return lRet;
                }

                bool fCallDWP = !DwmDefWindowProc(hwnd, msg, wParam, lParam, ref lRet);

                if (wmMsg == WM.NCHITTEST && lRet == IntPtr.Zero)
                {
                    //обработка нажатий мыши
                    lRet = HitTestNCA(hwnd, wParam, lParam);

                    if (lRet != (IntPtr)HT.NOWHERE)
                    {
                        fCallDWP = false;
                    }
                }

                //если сообщение не обработано, передаем базовой процедуре
                handled = !fCallDWP;
            }

            return lRet;
        }

        //обработка координат мыши для неклиентской области
        private IntPtr HitTestNCA(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            // Get the point coordinates for the hit test.
            var ptMouse = new Point(GetXLParam(lParam), GetYLParam(lParam));

            // Get the window rectangle.
            Win32Rect rcWindow;
            GetWindowRect(hWnd, out rcWindow);

            // Get the frame rectangle, adjusted for the style without a caption.
            Win32Rect rcFrame = new Win32Rect();
            AdjustWindowRectEx(ref rcFrame, WS.OVERLAPPEDWINDOW & ~WS.CAPTION, false, 0);

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
                { (IntPtr)HT.TOPLEFT, fOnResizeBorder? (IntPtr)HT.TOP : (IntPtr)HT.CAPTION, (IntPtr)HT.TOPRIGHT },
                { (IntPtr)HT.LEFT,  (IntPtr)HT.NOWHERE, (IntPtr)HT.RIGHT},
                { (IntPtr)HT.BOTTOMLEFT, (IntPtr)HT.BOTTOM, (IntPtr)HT.BOTTOMRIGHT },
            };

            return hitTests[uRow, uCol];
        }
    }
}
