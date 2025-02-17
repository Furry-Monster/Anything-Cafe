using Cysharp.Threading.Tasks;
using DG.Tweening;

public class DiscoverUI : PlaySceneComponent, IInitializable
{
    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DrawInLeft();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DrawBackLeft();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence DrawInLeft()
    {
        return DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
            })
            .Append(transform.DOLocalMoveX(-600, 0.5f));
    }

    private Sequence DrawBackLeft()
    {
        return DOTween.Sequence()
            .OnKill(() =>
            {
                gameObject.SetActive(false);
            })
            .Append(transform.DOLocalMoveX(-1700, 0.5f));
    }

    public void OnCloseClick()
    {
        _ = UIManager.Instance.CloseReactive(this);
    }
}
