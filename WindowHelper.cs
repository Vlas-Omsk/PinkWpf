using PinkWpf.NativeStructs;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PinkWpf
{
    public sealed partial class WindowHelper
    {
        public Window Window { get; }
        public IntPtr Hwnd { get; private set; }
        public HwndSource HwndSource { get; private set; }
        public bool IsInstalled { get; private set; }

        public WindowHelper(Window window)
        {
            Window = window;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SM smIndex);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out Win32Rect lpRect);

        [DllImport("user32.dll")]
        private static extern bool AdjustWindowRectEx(ref Win32Rect lpRect, WS dwStyle, bool bMenu, uint dwExStyle);

        [DllImport("dwmapi.dll")]
        private static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(uint smIndex);

        [DllImport("user32.dll")]
        private static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("User32.dll")]
        private static extern uint GetRawInputData(IntPtr hRawInput, RID uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Win32Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, MONITOR flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(HandleRef hmonitor, MONITORINFO info);

        public void Install()
        {
            if (IsInstalled)
                return;
            IsInstalled = true;

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
            HwndSource.AddHook(InputHookWndProc);
            HwndSource.AddHook(InfinityWindowWndProc);
        }

        public void HideCloseButton()
        {
            SetWindowLong(Hwnd, GWL.STYLE, GetWindowLong(Hwnd, GWL.STYLE) & ~(int)WS.SYSMENU);
        }

        public MONITORINFO GetCurrentMonitorInfo()
        {
            var monitor = MonitorFromWindow(Hwnd, MONITOR.DEFAULTTONEAREST);
            MONITORINFO info = new MONITORINFO();
            GetMonitorInfo(new HandleRef(null, monitor), info);
            return info;
        }
    }
}
