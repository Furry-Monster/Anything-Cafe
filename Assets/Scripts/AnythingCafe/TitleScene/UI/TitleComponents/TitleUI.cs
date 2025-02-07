using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TitleUI :
    TitleComponent,
    IInitializable
{
    [Header("General")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    public void Init() => gameObject.SetActive(true);

    public override async UniTask Open()
    {
        if (_sequence.IsActive()) _sequence.Kill();
        _sequence = DOTween.Sequence()
            .OnPlay(() =>
            {
                _canvasGroup.interactable = true;
                gameObject.SetActive(true);
            })
            .Append(_canvasGroup.DOFade(1, 0.5f));
        _sequence.Play();
        await _sequence.AsyncWaitForCompletion();
    }

    public void OnThanksClick()
    {

    }

    public void OnSupportClick()
    {

    }

    public void OnNoticeClick()
    {

    }

    public void OnAboutClick() => Application.OpenURL("https://www.google.com");

    public void OnNewGameClick()
    {

    }

    public void OnContinueClick()
    {

    }

    public void OnGalleryClick()
    {

    }

    public void OnSettingClick()
    {

    }

    public void OnExitClick()
    {
        // TODO: 打开提示框，询问是否退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
