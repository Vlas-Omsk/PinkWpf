using System.Windows.Media;
using System.Windows;
using System;

namespace PinkWpf
{
    public static class DependencyObjectExtensions
    {
        public static T FindParentControlOfType<T>(this DependencyObject self) where T : DependencyObject
        {
            if (!TryFindParentControlOfType<T>(self, out var parent))
                throw new Exception("Cannot find parent");

            return parent;
        }

        public static bool TryFindParentControlOfType<T>(this DependencyObject self, out T parent) where T : DependencyObject
        {
            while (true)
            {
                self = VisualTreeHelper.GetParent(self);

                if (self == null)
                    break;

                if (self.GetType().Name == "PopupRoot")
                    self = ((FrameworkElement)self).Parent;

                if (self is T selfAsT)
                {
                    parent = selfAsT;
                    return true;
                }
            }

            parent = null;
            return false;
        }

        public static T FindParentOrCurrentControlOfType<T>(this DependencyObject self) where T : DependencyObject
        {
            if (!TryFindParentOrCurrentControlOfType<T>(self, out var parent))
                throw new Exception("Cannot find parent");

            return parent;
        }

        public static bool TryFindParentOrCurrentControlOfType<T>(this DependencyObject self, out T parent) where T : DependencyObject
        {
            if (self is T selfAsT)
            {
                parent = selfAsT;
                return true;
            }

            return TryFindParentControlOfType(self, out parent);
        }
    }
}
