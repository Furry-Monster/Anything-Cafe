using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class CommandAttr : System.Attribute
    {
        public string Id { get; }

        public CommandAttr(string id = null)
        {
            Id = id;
        }
    }
}
