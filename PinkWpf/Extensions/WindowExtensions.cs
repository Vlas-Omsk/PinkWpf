using PinkWpf.Windows;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PinkWpf
{
    public static class WindowExtensions
    {
        private static Dictionary<Window, WindowContext> _windowContexts = new Dictionary<Window, WindowContext>();

        public static WindowContext GetContext(this Window window)
        {
            if (!_windowContexts.TryGetValue(window, out WindowContext windowContext))
                windowContext = _windowContexts[window] = new WindowContext(window);

            return windowContext;
        }

        public static T InstallModule<T>(this Window window) where T : IWindowModule, new()
        {
            return GetContext(window).InstallModule<T>();
        }

        public static void UninstallModule(this Window window, IWindowModule module)
        {
            if (!_windowContexts.TryGetValue(window, out WindowContext windowContext))
                throw new Exception("The context for the window could not be found");

            windowContext.UninstallModule(module);
        }
    }
}
