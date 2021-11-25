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

        public override string ToString()
        {
            return $"(Left: {Left}, Top: {Top}, Bottom: {Right}, Bottom: {Bottom})";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator Rect(Win32Rect point)
        {
            return new Rect(point.Left, point.Top, point.Size.Width, point.Size.Height);
        }

        public static bool operator ==(Win32Rect left, Win32Rect right)
        {
            return left.Left == right.Left && left.Top == right.Top && left.Right == right.Right && left.Bottom == right.Bottom;
        }

        public static bool operator !=(Win32Rect left, Win32Rect right)
        {
            return !(left == right);
        }
    }
}
