using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public int X;
        public int Y;

        public Win32Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"(X: {X}, Y: {Y})";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator Point(Win32Point point)
        {
            return new Point(point.X, point.Y);
        }

        public static bool operator ==(Win32Point left, Win32Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Win32Point left, Win32Point right)
        {
            return !(left == right);
        }
    }
}
