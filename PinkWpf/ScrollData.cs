namespace PinkWpf
{
    public sealed class ScrollData
    {
        public ScrollData(float offset, bool scrollByPage)
        {
            Offset = offset;
            ScrollByPage = scrollByPage;
        }

        public float Offset { get; }
        public bool ScrollByPage { get; }
    }
}
