using System;

namespace PinkWpf
{
    public class MouseWheelEventArgs : EventArgs
    {
        public bool IsScrollByPage { get; internal set; }
        public bool IsHorizontalScroll { get; internal set; }
        public float ScrollDelta { get; internal set; }
        public float WheelDelta { get; internal set; }

        public MouseWheelEventArgs()
        {
        }
    }
}
