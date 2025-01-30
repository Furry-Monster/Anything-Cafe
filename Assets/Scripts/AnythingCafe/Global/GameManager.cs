using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1998 // �첽����ȱ�� "await" �����������ͬ����ʽ����

public class GameManager : PersistentSingleton<GameManager>
{
    private List<GameObject> _validateIntegrityObjects; // TODO:���б����л�����ʾ��Inspector����У�����༭

    private async void Start()
    {
        try
        {
            await PrepareGame();
            await ValidateIntegrity();
            await ReadyTitleScene();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException customErrorException)
            {
                // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
                throw ex;
            }
            // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
            throw new CustomErrorException(ex.Message,
                new CustomErrorItem(ErrorSeverity.ForceQuit, ErrorCode.GameInitFailed));
        }
    }

    private async UniTask ValidateIntegrity()
    {
        // TODO: �����Ϸ��Դ������
    }

    private async UniTask PrepareGame()
    {
        // TODO: ׼����Ϸ��Դ,��ʼ����Ϸ
    }

    /// <summary>
    /// ��ȡ�����е� <see cref="TitleSceneHandler"/>,����<see cref="TitleSceneHandler.OnSceneLoad"/>�����Game��ʼ��
    /// </summary>
    /// <returns>�첽UniTask����</returns>
    private async UniTask ReadyTitleScene()
    {
        var component = GameObject.FindWithTag("SceneHandler")?.GetComponent<ISceneHandler>();
        if (component == null) return;
        await component.OnSceneLoad();
    }
}
