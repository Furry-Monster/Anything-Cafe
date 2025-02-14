using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class EventAttr : System.Attribute
    {
        public string Id { get; }

        public EventAttr(string id = null)
        {
            Id = id;
        }
    }
}
