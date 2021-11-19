using PinkWpf.NativeStructs;
using System;

namespace PinkWpf
{
    public class MouseInputEventArgs : EventArgs
    {
        public Win32Point Point { get; }

        public MouseInputEventArgs(Win32Point point)
        {
            Point = point;
        }
    }
}
