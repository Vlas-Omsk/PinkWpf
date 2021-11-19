using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    struct NCCALCSIZE_PARAMS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Win32Rect[] rgrc;
        public IntPtr lppos;
    }
}
