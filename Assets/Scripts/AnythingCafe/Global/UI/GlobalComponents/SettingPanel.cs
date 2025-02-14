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

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DropIn();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RiseBack();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    private Sequence DropIn()
    {
        return DOTween.Sequence()
            .OnPlay(() =>
            {
                gameObject.SetActive(true);
            })
            .Append(transform.DOLocalMoveY(0, 0.5f));
    }

    private Sequence RiseBack()
    {
        return DOTween.Sequence()
            .OnKill(() =>
            {
                gameObject.SetActive(false);
            })
            .Append(transform.DOLocalMoveY(1500, 0.5f));
    }

    public void OnBackButtonClick()
    {
        _ = UIManager.Instance.CloseGlobal<SettingPanel>();
    }

    public void OnSaveButtonClick()
    {
        // TODO: Save settings
    }
}
