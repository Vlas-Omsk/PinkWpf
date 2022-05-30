using System;

namespace PinkWpf.Windows
{
    internal static class ParamHelper
    {
        public static int LowWord(IntPtr lp)
        {
            return (short)(ulong)lp & 0xffff;
        }

        public static int HighWord(IntPtr lp)
        {
            return ((short)(((ulong)lp) >> 16)) & 0xffff;
        }
    }
}
