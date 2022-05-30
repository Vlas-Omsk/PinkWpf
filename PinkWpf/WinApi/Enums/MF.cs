using System;

namespace PinkWpf.WinApi
{
    [Flags]
    internal enum MF : uint
    {
        BYCOMMAND = 0x00000000,
        BYPOSITION = 0x00000400,

        BITMAP = 0x00000004,
        CHECKED = 0x00000008,
        DISABLED = 0x00000002,
        GRAYED = 0x00000001,
        HILITE = 0x00000080,
        MOUSESELECT = 0x00008000,
        OWNERDRAW = 0x00000100,
        POPUP = 0x00000010,
        SYSMENU = 0x00002000
    }
}
