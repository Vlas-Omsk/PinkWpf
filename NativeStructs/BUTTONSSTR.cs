﻿using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BUTTONSSTR
    {
        [MarshalAs(UnmanagedType.U2)]
        public RI_MOUSE usButtonFlags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonData;
    }
}
