using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method |
                    AttributeTargets.Property)]
    public class AutoAttr : System.Attribute
    {
        public AutoAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}