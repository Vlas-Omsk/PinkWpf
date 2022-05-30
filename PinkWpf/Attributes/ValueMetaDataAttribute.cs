using System;

namespace PinkWpf
{
    public class ValueMetaData : Attribute
    {
        public object Description { get; }

        public ValueMetaData()
        {
        }

        public ValueMetaData(object description)
        {
            Description = description;
        }

        public override string ToString()
        {
            return Description.ToString();
        }
    }
}
