using Cysharp.Threading.Tasks;

/// <summary>
/// Loading组件基类
/// </summary>
public class LoadingComponent : ReactiveComponent
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
