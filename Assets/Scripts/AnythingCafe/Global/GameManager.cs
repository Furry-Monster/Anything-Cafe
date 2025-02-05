using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : PersistentSingleton<GameManager>
{
    [HideInInspector] public bool IsFirstInTitleScene = true; // �Ƿ��ǵ�һ�ν���Title����

    private List<GameObject> _validateIntegrityObjects; // TODO:���б����л�����ʾ��Inspector����У�����༭

    private async void Start()
    {
        try
        {
            UIManager.Instance.Init();
            SoundManager.Instance.Init();

            await PrepareGame();
            await ValidateIntegrity();
            await ReadyTitleScene();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException customErrorException)
            {
                // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
                throw;
            }
            // ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
            throw new CustomErrorException($"[GameManager] can't start game, because of error: {ex.Message}",
                new CustomErrorItem(ErrorSeverity.ForceQuit, ErrorCode.GameInitFailed));
        }
    }

    private async UniTask PrepareGame()
    {
        // TODO: ׼����Ϸ��Դ,��ʼ����Ϸ
        CursorManager.Instance.Init();
    }

    private async UniTask ValidateIntegrity()
    {
        // TODO: �����Ϸ��Դ������
    }

    /// <summary>
    /// ��ȡ�����е� <see cref="TitleSceneHandler"/>,����<see cref="TitleSceneHandler.OnSceneLoad"/>�����Game��ʼ��
    /// </summary>
    /// <returns>�첽UniTask����</returns>
    private async UniTask ReadyTitleScene()
    {
        var component = GameObject.FindWithTag("SceneHandler").GetComponent<ISceneHandler>();
        if (component == null) return;
        await component.OnSceneLoad();
    }
}
