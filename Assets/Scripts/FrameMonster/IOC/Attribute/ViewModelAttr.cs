using System;

namespace FrameMonster.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class)]
    public class ViewModelAttr : System.Attribute
    {
        public string Id { get; }

        public ViewModelAttr(string id = null)
        {
            Id = id;
        }
    }
}
