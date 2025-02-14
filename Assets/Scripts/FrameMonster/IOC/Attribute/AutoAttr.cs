using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class AutoAttr : System.Attribute
    {
        public string Id { get; }

        public AutoAttr(string id = null)
        {
            Id = id;
        }
    }
}
