using Cysharp.Threading.Tasks;
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager :
    PersistentSingleton<GameSceneManager>,
    IInitializable
{
    private ISceneHandler _currentSceneHandler;
    private bool _isLoading;

    public float LoadingProgress { get; private set; }

    public event Action<SceneID> OnSceneLoadStart;
    public event Action<SceneID> OnSceneLoadComplete;
    public event Action<float> OnLoadingProgressChanged;

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneToLoad"> 场景ID </param>
    /// <param name="loadingName"></param>
    /// <returns> 异步操作 </returns>
    /// <exception cref="CustomErrorException"> 场景加载失败 </exception>
    public async UniTask LoadScene(SceneID sceneToLoad, string loadingName = null)
    {
        if (_isLoading)
        {
            Debug.LogWarning("[SceneManager] Please Don't Call LoadScene Method Multiple Times At Once!");
            return;
        }

        try
        {
            _isLoading = true;
            LoadingProgress = 0;
            OnSceneLoadStart?.Invoke(sceneToLoad);

            if (loadingName != null)
                await UIManager.Instance.OpenLoading(loadingName);

            await UnloadCurrentScene();
            await LoadNewScene(sceneToLoad);

            if (loadingName != null)
                await UIManager.Instance.OpenLoading(loadingName);

            OnSceneLoadComplete?.Invoke(sceneToLoad);
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException(
                $"[SceneManager] Failed to load scene: {sceneToLoad}",
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
        if (_currentSceneHandler != null)
        {
            try
            {
                await _currentSceneHandler.OnSceneUnload();
                _currentSceneHandler = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"场景卸载错误: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// 加载新的场景
    /// </summary>
    /// <param name="sceneId"></param>
    /// <returns></returns>
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
        await InitializeNewScene();
    }

    /// <summary>
    /// 场景相关资源的预加载(音频、预制体等)
    /// </summary>
    /// <returns></returns>
    private async UniTask PreloadSceneResources()
    {
        // TODO: 在这里实现场景相关资源的预加载
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 初始化新场景
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeNewScene()
    {
        var newSceneHandler = GameObject.FindWithTag("SceneHandler")?.GetComponent<ISceneHandler>();

        if (newSceneHandler != null)
        {
            _currentSceneHandler = newSceneHandler;
            await _currentSceneHandler.OnSceneLoad();
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) =>
        Debug.Log($"场景加载完成: {scene.name} in mode {mode}");


    private static void OnSceneUnloaded(Scene scene) =>
        Debug.Log($"场景卸载完成: {scene.name}");


    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        base.OnDestroy();
    }
}
