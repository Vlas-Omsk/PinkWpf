using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal struct MONITORINFO
    {
        public uint cbSize;
        public Win32Rect rcMonitor;
        public Win32Rect rcWork;
        public uint dwFlags;

        public MONITORINFO(Win32Rect rcMonitor, Win32Rect rcWork, uint dwFlags)
        {
            this.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
            this.rcMonitor = rcMonitor;
            this.rcWork = rcWork;
            this.dwFlags = dwFlags;
        }
    }
}
