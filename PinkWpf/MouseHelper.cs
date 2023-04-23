using PinkWpf.WinApi;

namespace PinkWpf
{
    public static class MouseHelper
    {
        public static ScrollData GetScrollData(float wheelDelta, ScrollType scrollType)
        {
            var scrollByPage = false;
            var scrollData = wheelDelta / Constants.WheelDelta;

            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    var scrollChars = Constants.ScrollCharsPerWheelDelta;
                    User32.SystemParametersInfo(SPI.GETWHEELSCROLLCHARS, 0, ref scrollChars, 0);
                    if (scrollChars == uint.MaxValue)
                        scrollByPage = true;
                    else
                        scrollData *= scrollChars;
                    break;
                case ScrollType.Vertical:
                    var scrollLines = Constants.ScrollLinesPerWheelDelta;
                    User32.SystemParametersInfo(SPI.GETWHEELSCROLLLINES, 0, ref scrollLines, 0);
                    if (scrollLines == uint.MaxValue)
                        scrollByPage = true;
                    else
                        scrollData *= scrollLines;
                    break;
            }

            return new ScrollData(scrollData, scrollByPage);
        }
    }
}
