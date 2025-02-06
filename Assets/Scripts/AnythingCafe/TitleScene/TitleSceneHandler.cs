using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{
    [Header("Audio")]
    [SerializeField] private string _titleBGM; // TODO: 从soundManager存储的Music中获取可用的Music枚举，并在Inspector上显示

    [Header("Layout")]
    [SerializeField]
    private GameObject _warningUI;
    [SerializeField]
    private GameObject _titleUI;
    [SerializeField]
    private GameObject _galleryUI;
    [SerializeField]
    private GameObject _settingUI;
    [SerializeField]
    private GameObject _thanksUI;
    [SerializeField]
    private GameObject _supportUI;

    public async UniTask OnSceneLoad()
    {
        try
        {
            await InitScene();
            await ShowTitleScript();
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
        // TODO: 初始化场景
    }

    private async UniTask ShowTitleScript()
    {
        // TODO: 显示标题场景
        if (GameManager.Instance.IsFirstInTitleScene)
        {
            // TODO：显示Warning



            GameManager.Instance.IsFirstInTitleScene = false;
        }


    }
}
