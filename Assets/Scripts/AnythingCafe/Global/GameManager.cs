using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            DOTween.Init(null, null, LogBehaviour.ErrorsOnly);
            // �˴������Զ���Tween������Ĭ�ϲ���

            OptionManager.Instance.Init();
            UIManager.Instance.Init();
            SoundManager.Instance.Init();
            CursorManager.Instance.Init();
            GameSceneManager.Instance.Init();

            await ReadyTitleScene();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException customErrorException)
            {
                // TODO:ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
                throw;
            }
            // TODO:ͨ��UIManager��UI������ʾ������Ϣ������ʾ�˳�
            throw new CustomErrorException($"[GameManager] can't start game, because of error: {ex.Message}",
                new CustomErrorItem(ErrorSeverity.ForceQuit, ErrorCode.GameInitFailed));
        }
    }

    public void OnApplicationQuit()
    {
        DOTween.Clear();
    }

    /// <summary>
    /// ��ȡ�����е� <see cref="TitleSceneHandler"/>,����<see cref="TitleSceneHandler.OnSceneLoad"/>�����Game��ʼ��
    /// </summary>
    /// <returns>�첽UniTask����</returns>
    private async UniTask ReadyTitleScene()
    {
        var component = GameSceneManager.Instance.CurrentSceneHandler;
        if (component == null) return;
        await component.OnSceneLoad();
    }
}
