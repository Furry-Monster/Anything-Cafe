using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method |
                    AttributeTargets.Property)]
    public class InjectAttr : System.Attribute
    {
        public InjectAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}