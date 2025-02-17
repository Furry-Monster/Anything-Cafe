using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SettingPanel :
    GlobalComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        gameObject.SetActive(false);
    }

    public void OnCloseClick()
    {
        _ = UIManager.Instance.CloseReactive(this);
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
        return DOTween.Sequence().OnPlay(() =>
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            _canvasGroup.interactable = true;
        }).Append(transform.DOLocalMoveY(0, 0.5f));
    }

    private Sequence RiseBack()
    {
        return DOTween.Sequence().OnPlay(() =>
        {
            _canvasGroup.interactable = false;
        }).OnKill(() =>
        {
            gameObject.SetActive(false);
        }).Append(transform.DOLocalMoveY(1700, 0.5f));
    }
}
