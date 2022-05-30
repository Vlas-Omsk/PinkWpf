using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    internal static class DwmApi
    {
        [DllImport("dwmapi.dll")]
        public static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);
    }
}
