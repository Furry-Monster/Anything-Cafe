using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class ControllerAttr : System.Attribute
    {
        public ControllerAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}