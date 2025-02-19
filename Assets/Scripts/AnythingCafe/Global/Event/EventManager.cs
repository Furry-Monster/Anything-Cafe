public class EventManager :
    PersistentSingleton<EventManager>,
    IInitializable
{
    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;

        // TODO: ÔØÈëÊÂ¼ş´æµµ

        IsInitialized = true;
    }
}
