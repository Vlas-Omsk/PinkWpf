using PinkWpf.NativeStructs;
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace PinkWpf
{
    public partial class WindowHelper
    {
        public bool InputHookEnabled { get; private set; }

        private Win32Point _mousePosition;
        private readonly RAWINPUTDEVICE[] _rawInputDevices =
        {
            new RAWINPUTDEVICE()
            {
                usUsagePage = HID.USAGE_PAGE_GENERIC,
                usUsage = HID.USAGE_GENERIC_MOUSE,
                dwFlags = RIDEV.INPUTSINK
            }
        };

        public void EnableInputHook()
        {
            if (InputHookEnabled)
                return;
            InputHookEnabled = true;

            for (var i = 0; i < _rawInputDevices.Length; i++)
                _rawInputDevices[i].hwndTarget = Hwnd;

            if (!RegisterRawInputDevices(_rawInputDevices, _rawInputDevices.Length, Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
                throw new Exception("Cant hook mouse (error: " + Marshal.GetLastWin32Error() + ")");
        }

        private IntPtr InputHookWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var wmMsg = (WM)msg;

            if (InputHookEnabled && wmMsg == WM.INPUT)
            {
                uint dwSize = 0;

                GetRawInputData(lParam, RID.INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
                IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);

                try
                {
                    if (buffer != IntPtr.Zero &&
                        GetRawInputData(lParam, RID.INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                    {
                        RAWINPUT rawInput = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));
                        if (rawInput.header.dwType == RIM.TYPEMOUSE)
                        {
                            if (rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE) ||
                                rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_RELATIVE))
                                ProcessMouseMove(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.WHEEL) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL))
                                ProcessMouseWheel(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                                ProcessMouseButtonDown(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
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

        private void ProcessMouseMove(RAWINPUT rawInput)
        {
            Win32Point point;
            if (rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE))
            {
                bool isVirtualDesktop = rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.VIRTUAL_DESKTOP);
                int width = GetSystemMetrics(isVirtualDesktop ? SM.CXVIRTUALSCREEN : SM.CXSCREEN);
                int height = GetSystemMetrics(isVirtualDesktop ? SM.CYVIRTUALSCREEN : SM.CYSCREEN);

                point = new Win32Point()
                {
                    X = (int)((rawInput.Mouse.lLastX / 65535.0f) * width),
                    Y = (int)((rawInput.Mouse.lLastY / 65535.0f) * height)
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
                RaiseMouseMove(new MouseInputEventArgs(point));
            }
        }

        private void ProcessMouseWheel(RAWINPUT rawInput)
        {
            var e = new MouseWheelEventArgs();

            e.WheelDelta = (short)rawInput.Mouse.buttons.usButtonData;
            e.IsHorizontalScroll = rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL);
            e.ScrollDelta = GetScrollDelta(e.WheelDelta, e.IsHorizontalScroll, out bool isScrollByPage);
            e.IsScrollByPage = isScrollByPage;

            RaiseMouseWheel(e);
        }

        private void ProcessMouseButtonDown(RAWINPUT rawInput)
        {
            var e = new MouseButtonEventArgs(_mousePosition);

            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN))
                e.Button = MouseButton.Left;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN))
                e.Button = MouseButton.Right;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN))
                e.Button = MouseButton.Middle;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN))
                e.Button = MouseButton.XButton1;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                e.Button = MouseButton.XButton2;

            RaiseMouseDown(e);
        }

        private void ProcessMouseButtonUp(RAWINPUT rawInput)
        {
            var e = new MouseButtonEventArgs(_mousePosition);

            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP))
                e.Button = MouseButton.Left;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP))
                e.Button = MouseButton.Right;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP))
                e.Button = MouseButton.Middle;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP))
                e.Button = MouseButton.XButton1;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
                e.Button = MouseButton.XButton2;

            RaiseMouseUp(e);
        }

        private void RaiseMouseMove(MouseInputEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void RaiseMouseWheel(MouseWheelEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }

        private void RaiseMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        private void RaiseMouseUp(MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        public event EventHandler<MouseInputEventArgs> MouseMove;
        public event EventHandler<MouseWheelEventArgs> MouseWheel;
        public event EventHandler<MouseButtonEventArgs> MouseDown, MouseUp;
    }
}
