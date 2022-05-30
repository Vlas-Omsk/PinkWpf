using PinkWpf.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PinkWpf
{
    public sealed class Monitor
    {
        public Int32Rect Area { get; private set; }
        public Int32Rect WorkArea { get; private set; }

        private IntPtr _handle;

        public Monitor(IntPtr handle)
        {
            _handle = handle;

            Update();
        }

        public void Update()
        {
            var info = new MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
            User32.GetMonitorInfo(_handle, ref info);

            Area = info.rcMonitor;
            WorkArea = info.rcWork;
        }

        public static Monitor FromWindow(Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            return FromWindow(windowInteropHelper.Handle);
        }

        internal static Monitor FromWindow(IntPtr hwnd)
        {
            var handle = User32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
            return new Monitor(handle);
        }
    }
}
