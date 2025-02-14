namespace FrameMonster.IOC
{
    public interface IContainer
    {
        public void Register<TInterface, TImplementation>(string id, bool isSingleton)
            where TImplementation : TInterface;

        public TInterface Resolve<TInterface>();

        public void UnregisterAll();
    }
}
