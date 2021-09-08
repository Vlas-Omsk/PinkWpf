using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;

namespace PinkWpf
{
    [StructLayout(LayoutKind.Sequential)]
#pragma warning disable CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
#pragma warning disable CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    public struct Win32Point
#pragma warning restore CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
#pragma warning restore CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
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

    public static class GlobalHook
    {
        public static bool IsInstalled { get; private set; }
        public static Win32Point MousePosition => _mousePosition;

        private static Win32Point _mousePosition;
        private static readonly RAWINPUTDEVICE[] _rawInputDevices =
        {
            new RAWINPUTDEVICE()
            {
                UsagePage = HID_USAGE_PAGE_GENERIC,
                Usage = HID_USAGE_GENERIC_MOUSE,
                Flags = RIDEV_INPUTSINK
            }
        };

        #region const
        const int WM_INPUT = 0x00FF;

        const int RID_INPUT = 0x10000003;

        const int RIM_TYPEMOUSE = 0;

        const int RIDEV_INPUTSINK = 0x00000100;

        const int HID_USAGE_PAGE_GENERIC = 0x01;

        const int HID_USAGE_GENERIC_MOUSE = 0x02;
        const int HID_USAGE_GENERIC_KEYBOARD = 0x06;

        const int SM_CXVIRTUALSCREEN = 78;
        const int SM_CYVIRTUALSCREEN = 79;
        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        const int WHEEL_DELTA = 120;

        const int SPI_GETWHEELSCROLLCHARS = 0x006C;
        const int SPI_GETWHEELSCROLLLINES = 0x0068;

        const uint defaultScrollLinesPerWheelDelta = 3;
        const uint defaultScrollCharsPerWheelDelta = 1;
        #endregion

        #region DLLImport
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("User32.dll")]
        private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int smIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Win32Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);
        #endregion

        #region struct
        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            /// <summary>Top level collection Usage page for the raw input device.</summary>
            public ushort UsagePage;
            /// <summary>Top level collection Usage for the raw input device. </summary>
            public ushort Usage;
            /// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary>
            public int Flags;
            /// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary>
            public IntPtr WindowHandle;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            [FieldOffset(16)]
            public RAWMOUSE mouse;
            [FieldOffset(16)]
            public RAWKEYBOARD keyboard;
            [FieldOffset(16)]
            public RAWHID hid;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int wParam;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public RAWMOUSE_FLAGS usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public BUTTONSSTR buttonsStr;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public RI_MOUSE usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }
        #endregion

        #region enum
        [Flags()]
        private enum RAWMOUSE_FLAGS : ushort
        {
            MOVE_RELATIVE = 0x00,
            MOVE_ABSOLUTE = 0x01,
            VIRTUAL_DESKTOP = 0x02,
            ATTRIBUTES_CHANGED = 0x04,
            MOVE_NOCOALESCE = 0x08
        }

        [Flags()]
        private enum RI_MOUSE : ushort
        {
            LEFT_BUTTON_DOWN = 0x0001,
            LEFT_BUTTON_UP = 0x0002,
            RIGHT_BUTTON_DOWN = 0x0004,
            RIGHT_BUTTON_UP = 0x0008,
            MIDDLE_BUTTON_DOWN = 0x0010,
            MIDDLE_BUTTON_UP = 0x0020,
            BUTTON_4_DOWN = 0x0040,
            BUTTON_4_UP = 0x0080,
            BUTTON_5_DOWN = 0x0100,
            BUTTON_5_UP = 0x0200,
            WHEEL = 0x0400,
            HWHEEL = 0x0800
        }
        #endregion

        public static void Install()
        {
            if (IsInstalled)
                return;

            var hwnd = Process.GetCurrentProcess().MainWindowHandle;
            if (hwnd == IntPtr.Zero)
            {
                DeferredInstall();
                return;
            }
            var hwndSource = HwndSource.FromHwnd(hwnd);
            if (hwndSource == null)
            {
                DeferredInstall();
                return;
            }

            InstallInternal(hwnd, hwndSource);
        }

        private static void DeferredInstall()
        {
            Application.Current.MainWindow.Loaded += (sender, e) =>
            {
                var hwnd = Process.GetCurrentProcess().MainWindowHandle;
                var hwndSource = HwndSource.FromHwnd(hwnd);
                InstallInternal(hwnd, hwndSource);
            };
        }

        private static void InstallInternal(IntPtr hwnd, HwndSource hwndSource)
        {
            hwndSource.AddHook(WndProc);

            for (var i = 0; i < _rawInputDevices.Length; i++)
                _rawInputDevices[i].WindowHandle = hwnd;

            if (!RegisterRawInputDevices(_rawInputDevices, _rawInputDevices.Length, Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
                throw new Exception("Cant hook mouse (error: " + Marshal.GetLastWin32Error() + ")");

            IsInstalled = true;
        }

        private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_INPUT)
            {
                uint dwSize = 0;

                GetRawInputData(lParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);

                try
                {
                    if (buffer != IntPtr.Zero &&
                        GetRawInputData(lParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                    {
                        RAWINPUT rawInput = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));
                        if (rawInput.header.dwType == RIM_TYPEMOUSE)
                        {
                            if (rawInput.mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE) ||
                                rawInput.mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_RELATIVE))
                                ProcessMouseMove(rawInput);
                            if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.WHEEL) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL))
                                ProcessMouseWheel(rawInput);
                            if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                                ProcessMouseButtonDown(rawInput);
                            if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP) ||
                                rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
                                ProcessMouseButtonUp(rawInput);
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            return IntPtr.Zero;
        }

        private static void ProcessMouseMove(RAWINPUT rawInput)
        {
            Win32Point point;
            if (rawInput.mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE))
            {
                bool isVirtualDesktop = rawInput.mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.VIRTUAL_DESKTOP);
                int width = GetSystemMetrics(isVirtualDesktop ? SM_CXVIRTUALSCREEN : SM_CXSCREEN);
                int height = GetSystemMetrics(isVirtualDesktop ? SM_CYVIRTUALSCREEN : SM_CYSCREEN);

                point = new Win32Point()
                {
                    X = (int)((rawInput.mouse.lLastX / 65535.0f) * width),
                    Y = (int)((rawInput.mouse.lLastY / 65535.0f) * height)
                };
            }
            else
            {
                if (!GetCursorPos(out point))
                    return;
            }
            if (point != _mousePosition)
            {
                _mousePosition = point;
                OnMouseMove(new MouseInputEventArgs(point));
            }
        }

        private static void ProcessMouseWheel(RAWINPUT rawInput)
        {
            var e = new MouseWheelEventArgs();

            e.WheelDelta = (float)(short)(rawInput.mouse.buttonsStr.usButtonData);
            float numTicks = e.WheelDelta / WHEEL_DELTA;

            e.IsHorizontalScroll = rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL);
            e.ScrollDelta = numTicks;

            if (e.IsHorizontalScroll)
            {
                var scrollChars = defaultScrollCharsPerWheelDelta;
                SystemParametersInfo(SPI_GETWHEELSCROLLCHARS, 0, ref scrollChars, 0);
                e.ScrollDelta *= scrollChars;
            }
            else
            {
                var scrollLines = defaultScrollLinesPerWheelDelta;
                SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, ref scrollLines, 0);
                if (scrollLines == uint.MaxValue)
                    e.IsScrollByPage = true;
                else
                    e.ScrollDelta *= scrollLines;
            }

            OnMouseWheel(e);
        }

        private static void ProcessMouseButtonDown(RAWINPUT rawInput)
        {
            var e = new MouseButtonEventArgs(_mousePosition);

            if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN))
                e.Button = MouseButton.Left;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN))
                e.Button = MouseButton.Right;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN))
                e.Button = MouseButton.Middle;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN))
                e.Button = MouseButton.XButton1;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                e.Button = MouseButton.XButton2;

            OnMouseDown(e);
        }

        private static void ProcessMouseButtonUp(RAWINPUT rawInput)
        {
            var e = new MouseButtonEventArgs(_mousePosition);

            if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP))
                e.Button = MouseButton.Left;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP))
                e.Button = MouseButton.Right;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP))
                e.Button = MouseButton.Middle;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP))
                e.Button = MouseButton.XButton1;
            else if (rawInput.mouse.buttonsStr.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
                e.Button = MouseButton.XButton2;

            OnMouseUp(e);
        }

        private static void OnMouseMove(MouseInputEventArgs e)
        {
            MouseMove?.Invoke(null, e);
        }

        private static void OnMouseWheel(MouseWheelEventArgs e)
        {
            MouseWheel?.Invoke(null, e);
        }

        private static void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(null, e);
        }

        private static void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(null, e);
        }

        public static event EventHandler<MouseInputEventArgs> MouseMove;
        public static event EventHandler<MouseWheelEventArgs> MouseWheel;
        public static event EventHandler<MouseButtonEventArgs> MouseDown, MouseUp;
    }

    public class MouseInputEventArgs : EventArgs
    {
        public Win32Point Point { get; }

        public MouseInputEventArgs(Win32Point point)
        {
            Point = point;
        }
    }

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

    public class MouseButtonEventArgs : MouseInputEventArgs
    {
        public MouseButton Button { get; internal set; }

        public MouseButtonEventArgs(Win32Point point) : base(point)
        {
        }
    }
}
