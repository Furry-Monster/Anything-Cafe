using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{

    [Header("Audio")]
    [SerializeField]
    private MusicSoundMeta _titleBGM;

    [Header("UI Layout")]
    [SerializeField]
    private WarningUI _warningUI;
    [SerializeField]
    private TitleUI _titleUI;
    [SerializeField]
    private ThanksUI _thanksUI;
    [SerializeField]
    private SupportUI _supportUI;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject _backGround;

    public async UniTask OnSceneLoad()
    {
        try
        {
            await InitScene();
            await ShowScene();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[TitleSceneHandler] Scene on load failed. Error message: {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneOnLoadFailed));
        }

    }

    public async UniTask OnSceneUnload()
    {

    }

    private async UniTask InitScene()
    {

    }

    private async UniTask ShowScene()
    {
        // 显示WarningUI，仅在第一次进入TitleScene时显示
        if (GameManager.Instance.IsFirstInTitleScene)
        {
#if !UNITY_EDITOR
            _warningUI.OnTextClosed += () => _backGround.SetActive(true);

            _backGround.SetActive(false);
            _titleUI.gameObject.SetActive(false);

            await _warningUI.Open();
            await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
            await _warningUI.Close();

            await _titleUI.Open();

            SoundManager.Instance.PlaySound(_titleBGM.Type, _titleBGM.Clip, _titleBGM.Loop, _titleBGM.DefaultVolume);

            GameManager.Instance.IsFirstInTitleScene = false;
#endif
        }

    }
}
