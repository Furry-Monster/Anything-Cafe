using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("FrameMonster/Scene/GameSceneManager")]
public class GameSceneManager :
    PersistentSingleton<GameSceneManager>,
    IInitializable
{
    private bool _isLoading;

    public ISceneHandler CurrentSceneHandler { get; private set; }

    public float LoadingProgress { get; private set; }

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

            // TODO: 在这里实现场景切换动画

            await UnloadCurrentScene();
            await LoadNewScene(sceneToLoad);
            await InitializeNewScene();

            // TODO: 关闭动画
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException(
                $"[GameSceneManager] Failed to load scene: {sceneToLoad},cause:{ex}",
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
    /// <returns> 异步操作 </returns>
    private async UniTask UnloadCurrentScene()
    {
        if (CurrentSceneHandler != null)
        {
            try
            {
                await CurrentSceneHandler.OnSceneUnload();
                CurrentSceneHandler = null;

                await Resources.UnloadUnusedAssets();
                GC.Collect();
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
        var sceneName = sceneId.ToString();
#if UNITY_EDITOR
        // 开发阶段EDITOR模式下,验证场景存在性
        if (!SceneExists(sceneName))
        {
            throw new CustomErrorException(
                $"[GameSceneManager] Scene {sceneName} not in build settings!",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneNotInBuild)
            );
        }
#endif

        var loadOperation = SceneManager.LoadSceneAsync(sceneName);
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
    }

#if UNITY_EDITOR
    private static bool SceneExists(string sceneName)
    {
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName) // 删除文件扩展名
                return true;
        }
        return false;
    }
#endif

    /// <summary>
    /// 场景相关资源的预加载(音频、预制体等)
    /// </summary>
    /// <returns> 异步操作 </returns>
    private async UniTask PreloadSceneResources()
    {
        await UniTask.CompletedTask;

        // TODO: 预加载场景相关资源
        // 
        // 以下代码为示例, 之后启用
        // 
        // const int totalSteps = 3;
        // var currentStep = 0;
        //
        // //预加载音频
        // currentStep++;
        // await Addressables.LoadAssetsAsync<AudioClip>("scene_audio", null)
        //     .ToUniTask(Progress.Create<float>(p =>
        //         UpdateCompositeProgress(p, currentStep, totalSteps)));
        //
        // //预加载预制体
        // currentStep++;
        // await Addressables.LoadAssetsAsync<GameObject>("scene_prefabs", null)
        //     .ToUniTask(Progress.Create<float>(p =>
        //         UpdateCompositeProgress(p, currentStep, totalSteps)));
        //
        // // 其他资源...
    }

    /// <summary>
    /// 更新任务组合进度
    /// </summary>
    /// <param name="partialProgress"> 进度百分比 </param>
    /// <param name="currentStep"> 当前步骤 </param>
    /// <param name="totalSteps"> </param>
    private void UpdateCompositeProgress(float partialProgress, int currentStep, int totalSteps)
    {
        var stepWeight = 1f / totalSteps;
        var baseProgress = (currentStep - 1) * stepWeight;
        LoadingProgress = baseProgress + partialProgress * stepWeight;
        OnLoadingProgressChanged?.Invoke(LoadingProgress);
    }

    /// <summary>
    /// 初始化新场景
    /// </summary>
    private async UniTask InitializeNewScene()
    {
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().isLoaded);

        var newSceneHandler = GameObject.FindWithTag("SceneHandler").GetComponent<ISceneHandler>();

        if (newSceneHandler != null)
        {
            CurrentSceneHandler = newSceneHandler;
            await CurrentSceneHandler.OnSceneLoad();
        }
    }
}
