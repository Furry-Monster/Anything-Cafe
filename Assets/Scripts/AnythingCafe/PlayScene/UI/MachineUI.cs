using Cysharp.Threading.Tasks;
using DG.Tweening;

public class MachineUI : PlaySceneComponent, IInitializable
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
        _sequence = DropDownPanel();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public override async UniTask Close()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = RiseUpPanel();
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public Sequence DropDownPanel()
    {
        return DOTween.Sequence();
    }

    public Sequence RiseUpPanel()
    {
        return DOTween.Sequence();
    }
}
