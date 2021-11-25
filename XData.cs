using System;

namespace PinkWpf
{
    public class XData<TXData> : NotifyPropertyChanged
        where TXData : XData<TXData>, new()
    {
        public static TXData Instance { get; } = new TXData();

        protected XData()
        {
        }
    }
}
