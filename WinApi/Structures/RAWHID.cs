using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWHID
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwSizeHid;
        [MarshalAs(UnmanagedType.U4)]
        public int dwCount;
        public byte bRawData;
    }
}
