using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        [MarshalAs(UnmanagedType.U4)]
        public RIM dwType;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }
}
