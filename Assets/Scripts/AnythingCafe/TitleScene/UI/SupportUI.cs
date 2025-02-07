using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SupportUI :
    TitleSceneComponent,
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

    private Sequence EmergeIn()
    {
        return DOTween.Sequence();
    }

    private Sequence EmergeOut()
    {
        return DOTween.Sequence();
    }

    public async UniTask OnCloseClick()
    {
        await UIManager.Instance.CloseReactive(this);
    }
}
