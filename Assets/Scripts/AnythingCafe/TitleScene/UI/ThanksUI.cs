using Cysharp.Threading.Tasks;
using DG.Tweening;

public class ThanksUI : TitleComponent
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

    private Sequence PopUpSequence()
    {
        return DOTween.Sequence();
    }

    private Sequence PopDownSequence()
    {
        return DOTween.Sequence();
    }
}
