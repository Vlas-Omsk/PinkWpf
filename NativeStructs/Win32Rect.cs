using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rect
    {
        public int Left, Top, Right, Bottom;

        public Size Size => new Size(Right - Left, Bottom - Top);
    }
}
