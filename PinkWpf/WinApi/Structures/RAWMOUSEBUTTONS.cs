using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWMOUSEBUTTONS
    {
        [MarshalAs(UnmanagedType.U2)]
        public RI_MOUSE usButtonFlags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonData;
    }
}
