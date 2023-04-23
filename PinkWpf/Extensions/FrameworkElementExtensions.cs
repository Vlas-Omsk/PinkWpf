using System.Windows;

namespace PinkWpf
{
    public static class FrameworkElementExtensions
    {
        public static T FindResource<T>(this FrameworkElement self, object resourceKey)
        {
            return (T)self.FindResource(resourceKey);
        }
    }
}
