using System;

namespace PinkWpf
{
    public abstract class XDataBase<TXData> : NotifyPropertyChanged
        where TXData : XDataBase<TXData>, new()
    {
        public static TXData Instance { get; private set; }

        protected XDataBase()
        {
        }

        public static void Initialize()
        {
            if (XDataGlobal.Instance != null)
                throw new Exception("XData already defined");
            XDataGlobal.Instance = Instance = new TXData();
        }
    }

    public static class XDataGlobal
    {
        public static object Instance { get; internal set; }
    }
}
