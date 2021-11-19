using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public Win32Rect rcMonitor;
        public Win32Rect rcWork;
        public int dwFlags;
    }
}
