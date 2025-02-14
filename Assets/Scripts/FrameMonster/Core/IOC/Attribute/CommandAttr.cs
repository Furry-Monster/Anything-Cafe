using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class CommandAttr : System.Attribute
    {
        public CommandAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}