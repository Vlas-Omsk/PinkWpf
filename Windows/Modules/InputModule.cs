using PinkWpf.WinApi;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace PinkWpf.Windows
{
    public sealed class InputModule : WindowModule
    {
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

        protected internal override void Install(WindowHelper helper)
        {
            for (var i = 0; i < _rawInputDevices.Length; i++)
                _rawInputDevices[i].hwndTarget = helper.Hwnd;

            if (!User32.RegisterRawInputDevices(_rawInputDevices, _rawInputDevices.Length, Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
                throw new Exception("Cant hook mouse (error: " + Marshal.GetLastWin32Error() + ")");
        }

        protected internal override void Dispose()
        {
        }

        protected internal override IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var wmMsg = (WM)msg;

            if (wmMsg == WM.INPUT)
            {
                var dwSize = 0u;

                User32.GetRawInputData(lParam, RID.INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
                var buffer = Marshal.AllocHGlobal((int)dwSize);

                try
                {
                    if (buffer != IntPtr.Zero && User32.GetRawInputData(lParam, RID.INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) == dwSize)
                    {
                        var rawInput = Marshal.PtrToStructure<RAWINPUT>(buffer);
                        if (rawInput.header.dwType == RIM.TYPEMOUSE)
                        {
                            if (rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE) ||
                                rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_RELATIVE))
                                HandleMouseMove(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.WHEEL) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL))
                                HandleMouseWheel(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                                HandleMouseButtonDown(rawInput);
                            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP) ||
                                rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
                                HandleMouseButtonUp(rawInput);
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

        private void HandleMouseMove(RAWINPUT rawInput)
        {
            Win32Point point;

            if (rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.MOVE_ABSOLUTE))
            {
                bool isVirtualDesktop = rawInput.Mouse.usFlags.HasFlag(RAWMOUSE_FLAGS.VIRTUAL_DESKTOP);
                int width = User32.GetSystemMetrics(isVirtualDesktop ? SM.CXVIRTUALSCREEN : SM.CXSCREEN);
                int height = User32.GetSystemMetrics(isVirtualDesktop ? SM.CYVIRTUALSCREEN : SM.CYSCREEN);

                point = new Win32Point()
                {
                    X = (int)(rawInput.Mouse.lLastX / 65535.0f * width),
                    Y = (int)(rawInput.Mouse.lLastY / 65535.0f * height)
                };
            }
            else if (!User32.GetCursorPos(out point))
            {
                return;
            }

            if (point != _mousePosition)
            {
                _mousePosition = point;
                RaiseMouseMove(new MouseInputEventArgs(point));
            }
        }

        private void HandleMouseWheel(RAWINPUT rawInput)
        {
            var wheelDelta = (short)rawInput.Mouse.buttons.usButtonData;
            var scrollType = rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.HWHEEL) ? ScrollType.Horizontal : ScrollType.Vertical;
            var scrollDelta = MouseHelper.GetScrollData(wheelDelta, scrollType, out bool scrollByPage);

            RaiseMouseWheel(new MouseWheelEventArgs(scrollByPage, scrollType, scrollDelta, wheelDelta));
        }

        private void HandleMouseButtonDown(RAWINPUT rawInput)
        {
            var button = (MouseButton)(-1);

            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_DOWN))
                button = MouseButton.Left;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_DOWN))
                button = MouseButton.Right;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_DOWN))
                button = MouseButton.Middle;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_DOWN))
                button = MouseButton.XButton1;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_DOWN))
                button = MouseButton.XButton2;

            RaiseMouseDown(new MouseButtonEventArgs(_mousePosition, button));
        }

        private void HandleMouseButtonUp(RAWINPUT rawInput)
        {
            var button = (MouseButton)(-1);

            if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.LEFT_BUTTON_UP))
                button = MouseButton.Left;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.RIGHT_BUTTON_UP))
                button = MouseButton.Right;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.MIDDLE_BUTTON_UP))
                button = MouseButton.Middle;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_4_UP))
                button = MouseButton.XButton1;
            else if (rawInput.Mouse.buttons.usButtonFlags.HasFlag(RI_MOUSE.BUTTON_5_UP))
                button = MouseButton.XButton2;

            RaiseMouseUp(new MouseButtonEventArgs(_mousePosition, button));
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

    public class MouseButtonEventArgs : MouseInputEventArgs
    {
        public MouseButton Button { get; }

        public MouseButtonEventArgs(Point point, MouseButton button) : base(point)
        {
            Button = button;
        }
    }

    public class MouseInputEventArgs : EventArgs
    {
        public Point Point { get; }

        public MouseInputEventArgs(Point point)
        {
            Point = point;
        }
    }

    public class MouseWheelEventArgs : EventArgs
    {
        public bool ScrollByPage { get; }
        public ScrollType ScrollType { get; }
        public float ScrollDelta { get; }
        public float WheelDelta { get; }

        public MouseWheelEventArgs(bool scrollByPage, ScrollType scrollType, float scrollDelta, float wheelDelta)
        {
            ScrollByPage = scrollByPage;
            ScrollType = scrollType;
            ScrollDelta = scrollDelta;
            WheelDelta = wheelDelta;
        }
    }
}
