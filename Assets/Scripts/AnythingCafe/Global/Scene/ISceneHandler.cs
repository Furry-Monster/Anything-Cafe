using Cysharp.Threading.Tasks;

public interface ISceneHandler
{
    public UniTask OnSceneLoad();

    public UniTask OnSceneUnload();
}
