using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SupportUI :
    TitleSceneComponent,
    IInitializable
{
    [Header("Components")]
    [SerializeField] private Button _closeBtn;

    private Sequence _sequence;

    public void Init()
    {
        gameObject.SetActive(false);

        _closeBtn.onClick.AddListener(() => _ = OnCloseClick());
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = EmergeIn();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = EmergeOut();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence EmergeIn()
    {
        return DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
            })
            .Append(transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack));
    }

    private Sequence EmergeOut()
    {
        return DOTween.Sequence()
            .OnKill(() =>
            {
                gameObject.SetActive(false);
            })
            .Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
    }

    public async UniTask OnCloseClick() =>
        await UIManager.Instance.CloseReactive(this);

}
