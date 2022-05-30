using System;

namespace PinkWpf.WinApi
{
    internal enum WM : int
    {
        INPUT = 0x00FF,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCACTIVATE = 0x0086,
        SYSCOMMAND = 0x0112,
        MENUSELECT = 0x011F,
        INITMENUPOPUP = 0x0117,
        COMMAND = 0x0111,
        EXITMENULOOP = 0x0212
    }
}
