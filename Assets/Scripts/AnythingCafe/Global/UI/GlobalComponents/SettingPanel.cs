using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SettingPanel :
    GlobalComponent,
    IInitializable
{
    private Sequence _sequence;

    public void Init() => gameObject.SetActive(false);

    public override UniTask Open()
    {
        return base.Open();
    }

    public override UniTask Close()
    {
        return base.Close();
    }

    private Sequence DropIn()
    {
        return DOTween.Sequence();
    }

    private Sequence RiseBack()
    {
        return DOTween.Sequence();
    }
}
