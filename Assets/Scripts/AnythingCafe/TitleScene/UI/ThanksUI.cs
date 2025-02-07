using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ThanksUI : 
    TitleSceneComponent,
    IInitializable
{
    private RectTransform _thanksPanel;

    private Sequence _sequence;

    public void Init()
    {
        gameObject.SetActive(false);

        _thanksPanel = GetComponent<RectTransform>();
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RightPopUpSequence();
        _sequence.Play();

        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RightPopDownSequence();
        _sequence.Play();

        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence RightPopUpSequence()
    {
        return DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
            })
            .Append(_thanksPanel.DOAnchorPosX(0, 0.5f));
    }

    private Sequence RightPopDownSequence()
    {
        return DOTween.Sequence()
            .OnKill(() =>
            {
                gameObject.SetActive(false);
            })
            .Append(_thanksPanel.DOAnchorPosX(1000, 0.5f));
    }
}
