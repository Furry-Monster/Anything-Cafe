using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class SystemAttr : Attribute
    {
        public SystemAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}