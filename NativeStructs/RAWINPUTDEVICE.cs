using System;
using System.Runtime.InteropServices;

namespace PinkWpf.NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICE
    {
        /// <summary>Top level collection Usage page for the raw input device.</summary>
        public HID usUsagePage;
        /// <summary>Top level collection Usage for the raw input device. </summary>
        public HID usUsage;
        /// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary>
        public RIDEV dwFlags;
        /// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary>
        public IntPtr hwndTarget;
    }
}
