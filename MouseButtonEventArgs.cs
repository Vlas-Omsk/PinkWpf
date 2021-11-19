using PinkWpf.NativeStructs;
using System;
using System.Windows.Input;

namespace PinkWpf
{
    public class MouseButtonEventArgs : MouseInputEventArgs
    {
        public MouseButton Button { get; internal set; }

        public MouseButtonEventArgs(Win32Point point) : base(point)
        {
        }
    }
}
