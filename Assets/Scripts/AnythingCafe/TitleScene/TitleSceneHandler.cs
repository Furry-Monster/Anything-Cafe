using Cysharp.Threading.Tasks;
using System;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{
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
    }

    private async UniTask ShowTitleScene()
    {
        // TODO: ��ʾ���ⳡ��
    }
}
