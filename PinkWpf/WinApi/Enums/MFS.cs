using System;

namespace PinkWpf.WinApi
{
    [Flags]
    internal enum MFS : uint
    {
        CHECKED = 0x00000008,
        DEFAULT = 0x00001000,
        DISABLED = 0x00000003,
        HILITE = 0x00000080
    }
}
