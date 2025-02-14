namespace FrameMonster.Core.IOC
{
    public interface IContext
    {
        public void Init();

        public void RegisterCommand<TInterface, TImpl>();
        public void RegisterController<TInterface, TImpl>();
        public void RegisterSystem<TInterface, TImpl>();
        public void RegisterModel<TInterface, TImpl>();
        public void RegisterEvent<TInterface, TImpl>();

        public TInterface ResolveCommand<TInterface>();
        public TInterface ResolveController<TInterface>();
        public TInterface ResolveSystem<TInterface>();
        public TInterface ResolveModel<TInterface>();
        public TInterface ResolveEvent<TInterface>();

        public void SendCommand<T>(T command);
        public TResult SendQuery<T, TResult>(T query);

        public void PublishEvent<T>(T @event);

        public void Purge();
    }
}
