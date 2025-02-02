using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{
    private bool _isFirstEnter = true;

    [Header("Audio")]
    [SerializeField] private string _titleBGM; // TODO: ��soundManager�洢��Music�л�ȡ���õ�Musicö�٣�����Inspector����ʾ

    public async UniTask OnSceneLoad()
    {
        try
        {
            await InitScene();
            await ShowTitleScene();
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
        if (_isFirstEnter)
        {
            _isFirstEnter = false;
        }
    }

    private async UniTask ShowTitleScene()
    {
        // TODO: ��ʾ���ⳡ��
    }
}
