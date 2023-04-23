using System.Windows;

namespace PinkWpf.GridDefinitionsMarkup
{
    internal sealed class FrameworkElementResourceLocatorAccessor : IResourceLocator
    {
        private readonly FrameworkElement _element;

        public FrameworkElementResourceLocatorAccessor(FrameworkElement element)
        {
            _element = element;
        }

        public object FindResource(object resourceKey)
        {
            return _element.FindResource(resourceKey);
        }
    }
}
