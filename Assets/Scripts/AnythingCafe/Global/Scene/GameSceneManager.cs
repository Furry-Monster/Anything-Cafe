using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("FrameMonster/Scene/SceneManager")]
public class GameSceneManager :
    PersistentSingleton<GameSceneManager>,
    IInitializable
{
    private bool _isLoading;

    public ISceneHandler CurrentSceneHandler { get; private set; }

    public float LoadingProgress { get; private set; }

    public event Action<SceneID> OnSceneLoadStart;
    public event Action<SceneID> OnSceneLoadComplete;
    public event Action<float> OnLoadingProgressChanged;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        
        CurrentSceneHandler ??= GameObject.FindWithTag("SceneHandler").GetComponent<ISceneHandler>();

        IsInitialized = true;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneToLoad"> 场景ID </param>
    /// <param name="loadingName"> 加载场景时的文字提示 </param>
    /// <returns> 异步操作 </returns>
    /// <exception cref="CustomErrorException"> 场景加载失败 </exception>
    public async UniTask LoadScene(SceneID sceneToLoad, string loadingName = null)
    {
        if (_isLoading)
        {
            Debug.LogWarning("[GameSceneManager] Please Don't Call LoadScene Method Multiple Times At Once!");
            return;
        }

        try
        {
            _isLoading = true;
            LoadingProgress = 0;
            OnSceneLoadStart?.Invoke(sceneToLoad);

            // TODO: 在这里实现场景切换动画

            await UnloadCurrentScene();
            await LoadNewScene(sceneToLoad);

            // TODO: 关闭动画

            OnSceneLoadComplete?.Invoke(sceneToLoad);
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException(
                $"[GameSceneManager] Failed to load scene: {sceneToLoad}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneCantLoad)
            );
        }
        finally
        {
            _isLoading = false;
            LoadingProgress = 1;
        }
    }

    /// <summary>
    /// 卸载当前场景
    /// </summary>
    /// <returns></returns>
    private async UniTask UnloadCurrentScene()
    {
        if (CurrentSceneHandler != null)
        {
            try
            {
                await CurrentSceneHandler.OnSceneUnload();
                CurrentSceneHandler = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameSceneManager] Scene unload error: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// 加载新的场景
    /// </summary>
    /// <param name="sceneId"> 场景ID </param>
    /// <returns> 异步操作 </returns>
    private async UniTask LoadNewScene(SceneID sceneId)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneId.ToString());
        loadOperation.allowSceneActivation = false;

        // 监控加载进度
        while (loadOperation.progress < 0.9f)
        {
            LoadingProgress = loadOperation.progress;
            OnLoadingProgressChanged?.Invoke(LoadingProgress);
            await UniTask.Yield();
        }

        await PreloadSceneResources();

        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
            await UniTask.Yield();
        
        InitializeNewScene();
    }

    /// <summary>
    /// 场景相关资源的预加载(音频、预制体等)
    /// </summary>
    /// <returns> 异步操作 </returns>
    private async UniTask PreloadSceneResources()
    {
        // TODO: 在这里实现场景相关资源的预加载
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 初始化新场景
    /// </summary>
    private void InitializeNewScene()
    {
        var newSceneHandler = GameObject.FindWithTag("SceneHandler")?.GetComponent<ISceneHandler>();

        if (newSceneHandler != null)
        {
            CurrentSceneHandler = newSceneHandler;
        }
    }
}
