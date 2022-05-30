using System;

namespace PinkWpf.WinApi
{
    [Flags]
    internal enum MFT : uint
    {
        MENUBARBREAK = 0x00000020,
        MENUBREAK = 0x00000040,
        OWNERDRAW = 0x00000100,
        RADIOCHECK = 0x00000200,
        RIGHTJUSTIFY = 0x00004000,
        RIGHTORDER = 0x00002000,
        SEPARATOR = 0x00000800
    }
}
