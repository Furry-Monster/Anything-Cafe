using Cysharp.Threading.Tasks;

public class TitleComponent : ReactiveComponent
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
