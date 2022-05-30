using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTDEVICE
    {
        public HID usUsagePage;
        public HID usUsage;
        public RIDEV dwFlags;
        public IntPtr hwndTarget;
    }
}
