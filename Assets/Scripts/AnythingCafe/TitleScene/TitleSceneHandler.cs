using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{
    [Header("Audio")]
    [SerializeField] private string _titleBGM; // TODO: ��soundManager�洢��Music�л�ȡ���õ�Musicö�٣�����Inspector����ʾ

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
        // TODO: ��ʼ������
    }

    private async UniTask ShowTitleScript()
    {
        // TODO: ��ʾ���ⳡ��
        if (GameManager.Instance.IsFirstInTitleScene)
        {
            // TODO����ʾWarning



            GameManager.Instance.IsFirstInTitleScene = false;
        }


    }
}
