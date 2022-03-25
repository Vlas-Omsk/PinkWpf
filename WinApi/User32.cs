using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll")]
        public static extern long SetWindowLong(IntPtr hWnd, GWL nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SM smIndex);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out Win32Rect lpRect);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx(ref Win32Rect lpRect, WS dwStyle, bool bMenu, uint dwExStyle);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(uint smIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("User32.dll")]
        public static extern uint GetRawInputData(IntPtr hRawInput, RID uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Win32Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, MONITOR flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, MONITORINFO info);
    }
}
