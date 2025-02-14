using System;

namespace FrameMonster.Core.IOC
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class ModelAttr : System.Attribute
    {
        public ModelAttr(string id = null)
        {
            Id = id;
        }

        public string Id { get; }
    }
}