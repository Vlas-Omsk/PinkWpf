using System.Windows;

namespace PinkWpf.GridDefinitionsMarkup
{
    internal sealed class GridDefinition
    {
        public GridDefinition(bool isGap, GridLength? size, double? minSize, double? maxSize)
        {
            IsGap = isGap;
            Size = size;
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public bool IsGap { get; }
        public GridLength? Size { get; }
        public double? MinSize { get; }
        public double? MaxSize { get; }
    }
}
