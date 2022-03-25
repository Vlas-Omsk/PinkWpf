using System;
using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct RAWMOUSE
    {
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(0)]
        public RAWMOUSE_FLAGS usFlags;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(4)]
        public uint ulButtons;
        [FieldOffset(4)]
        public RAWMOUSEBUTTONS buttons;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(8)]
        public uint ulRawButtons;
        [FieldOffset(12)]
        public int lLastX;
        [FieldOffset(16)]
        public int lLastY;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public uint ulExtraInformation;
    }
}
