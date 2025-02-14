using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttr : System.Attribute
    {
        public string Id { get; }

        public InjectAttr(string id = null)
        {
            Id = id;
        }
    }
}
