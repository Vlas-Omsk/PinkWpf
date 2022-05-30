using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinkWpf
{
    public abstract class StoreBase<TStore>
        where TStore : StoreBase<TStore>, new()
    {
        public static TStore Instance { get; private set; }

        private readonly string _key;

        protected StoreBase(string key)
        {
            if (StoreGlobal.Instances.ContainsKey(key))
                throw new Exception($"Store with key \"{key}\" already defined");

            _key = key;
        }

        protected StoreBase() : this(StoreGlobal.DefaultInstanceKey)
        {
        }

        public static void Initialize()
        {
            if (Instance != null)
                throw new Exception($"Store already initialized");
            Instance = new TStore();
            StoreGlobal.Instances[Instance._key] = Instance;
        }
    }

    public static class StoreGlobal
    {
        public const string DefaultInstanceKey = "Default";
        public static Dictionary<string, object> Instances { get; } = new Dictionary<string, object>(1);
    }
}
