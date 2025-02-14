using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class ModelAttr : System.Attribute
    {
        public string Id { get; }

        public ModelAttr(string id = null)
        {
            Id = id;
        }
    }
}
