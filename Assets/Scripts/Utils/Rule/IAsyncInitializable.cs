using Cysharp.Threading.Tasks;

public interface IAsyncInitializable
{
    public bool IsInitialized { get; set; }

    public UniTask Init();
}
