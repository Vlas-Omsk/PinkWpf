using System.Runtime.InteropServices;

namespace PinkWpf.WinApi
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref uint pvParam, uint fWinIni);
    }
}
