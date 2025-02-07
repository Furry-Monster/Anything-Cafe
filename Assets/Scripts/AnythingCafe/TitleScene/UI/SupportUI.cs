using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SupportUI : TitleSceneComponent
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
}
