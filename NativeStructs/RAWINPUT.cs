using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER header;
        [FieldOffset(16)]
        public RAWMOUSE mouse32;
        [FieldOffset(24)]
        public RAWMOUSE mouse64;
        [FieldOffset(16)]
        public RAWKEYBOARD keyboard32;
        [FieldOffset(24)]
        public RAWKEYBOARD keyboard64;
        [FieldOffset(16)]
        public RAWHID hid32;
        [FieldOffset(24)]
        public RAWHID hid64;

        public RAWMOUSE Mouse => IntPtr.Size == 8 ? mouse64 : mouse32;
        public RAWKEYBOARD Keyboard => IntPtr.Size == 8 ? keyboard32 : keyboard64;
        public RAWHID Hid => IntPtr.Size == 8 ? hid32 : hid64;
    }
}
