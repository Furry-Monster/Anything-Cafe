public class EventManager :
    PersistentSingleton<EventManager>,
    IInitializable
{
    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;

        // TODO: �����¼��浵

        IsInitialized = true;
    }
}
