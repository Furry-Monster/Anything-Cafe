using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : PersistentSingleton<GameManager>
{
    private List<GameObject> _validateIntegrityObjects; // TODO:将列表序列化，显示在Inspector面板中，方便编辑

    // 只会在游戏启动时被调用一次，因为该类继承自可持久化单例
    protected override void Awake()
    {
        base.Awake();

        // TODO:初始化必需部分(加载语言本地化，调整分辨率适配，初始化UIManager，初始化AudioManager，播放音乐并显示加载界面)
        UIManager.Instance.Init();
        SoundManager.Instance.Init();
    }

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
                // 通过UIManager打开UI弹窗显示错误信息，并提示退出
                throw;
            }
            // 通过UIManager打开UI弹窗显示错误信息，并提示退出
            throw new CustomErrorException($"[GameManager] can't start game, because of error: {ex.Message}",
                new CustomErrorItem(ErrorSeverity.ForceQuit, ErrorCode.GameInitFailed));
        }
    }

    private async UniTask ValidateIntegrity()
    {
        // TODO: 检测游戏资源完整性
    }

    private async UniTask PrepareGame()
    {
        // TODO: 准备游戏资源,初始化游戏
    }

    /// <summary>
    /// 获取场景中的 <see cref="TitleSceneHandler"/>,启动<see cref="TitleSceneHandler.OnSceneLoad"/>，完成Game初始化
    /// </summary>
    /// <returns>异步UniTask任务</returns>
    private async UniTask ReadyTitleScene()
    {
        var component = GameObject.FindWithTag("SceneHandler").GetComponent<ISceneHandler>();
        if (component == null) return;
        await component.OnSceneLoad();
    }
}
