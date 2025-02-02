using Cysharp.Threading.Tasks;

/// <summary>
/// Global组件基类
/// </summary>
public class GlobalComponent : ReactiveComponent
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
