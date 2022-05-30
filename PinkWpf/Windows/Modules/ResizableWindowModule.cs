using PinkWpf.WinApi;
using System;
using System.Windows;

namespace PinkWpf.Windows
{
    public sealed class ResizableWindowModule : IWindowModule
    {
        private int _xborder;
        private int _yborder;

        void IWindowModule.Install(WindowContext context)
        {
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

            if (wmMsg == WM.NCHITTEST && lRet == IntPtr.Zero)
            {
                lRet = HitTestNCA(hwnd, wParam, lParam);

                if (lRet != (IntPtr)HT.NOWHERE)
                    handled = true;
            }

            return lRet;
        }

        // Обработка координат мыши для неклиентской области
        private IntPtr HitTestNCA(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            // Get the point coordinates for the hit test
            var ptMouse = new Point(ParamHelper.LowWord(lParam), ParamHelper.HighWord(lParam));

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
