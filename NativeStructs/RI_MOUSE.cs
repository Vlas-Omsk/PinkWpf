using System;

namespace PinkWpf.NativeStructs
{
    [Flags()]
    public enum RI_MOUSE : ushort
    {
        LEFT_BUTTON_DOWN = 0x0001,
        LEFT_BUTTON_UP = 0x0002,
        RIGHT_BUTTON_DOWN = 0x0004,
        RIGHT_BUTTON_UP = 0x0008,
        MIDDLE_BUTTON_DOWN = 0x0010,
        MIDDLE_BUTTON_UP = 0x0020,
        BUTTON_4_DOWN = 0x0040,
        BUTTON_4_UP = 0x0080,
        BUTTON_5_DOWN = 0x0100,
        BUTTON_5_UP = 0x0200,
        WHEEL = 0x0400,
        HWHEEL = 0x0800
    }
}
