using System;

namespace PinkWpf.NativeStructs
{
    [Flags()]
    public enum RAWMOUSE_FLAGS : ushort
    {
        MOVE_RELATIVE = 0x00,
        MOVE_ABSOLUTE = 0x01,
        VIRTUAL_DESKTOP = 0x02,
        ATTRIBUTES_CHANGED = 0x04,
        MOVE_NOCOALESCE = 0x08
    }
}
