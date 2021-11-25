using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWHID
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwSizeHid;
        [MarshalAs(UnmanagedType.U4)]
        public int dwCount;
        public byte bRawData;
    }
}
