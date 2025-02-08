using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SettingPanel :
    GlobalComponent,
    IInitializable
{
    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

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
