using Cysharp.Threading.Tasks;

public class PlaySceneComponent : ReactiveComponent
{
    public override UniTask Open()
    {
        this.gameObject.SetActive(true);
        return new UniTask();
    }

    public override UniTask Close()
    {
        this.gameObject.SetActive(false);
        return new UniTask();
    }
}
