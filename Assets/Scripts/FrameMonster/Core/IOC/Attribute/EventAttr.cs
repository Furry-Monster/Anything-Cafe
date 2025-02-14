using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class EventAttr : System.Attribute
    {
        public EventAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}