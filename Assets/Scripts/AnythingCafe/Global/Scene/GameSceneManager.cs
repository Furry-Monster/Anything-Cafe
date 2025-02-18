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
    /// ���س���
    /// </summary>
    /// <param name="sceneToLoad"> ����ID </param>
    /// <param name="loadingName"> ���س���ʱ��������ʾ </param>
    /// <returns> �첽���� </returns>
    /// <exception cref="CustomErrorException"> ��������ʧ�� </exception>
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

            // TODO: ������ʵ�ֳ����л�����

            await UnloadCurrentScene();
            await LoadNewScene(sceneToLoad);
            await InitializeNewScene();

            // TODO: �رն���
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
    /// ж�ص�ǰ����
    /// </summary>
    /// <returns> �첽���� </returns>
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
    /// �����µĳ���
    /// </summary>
    /// <param name="sceneId"> ����ID </param>
    /// <returns> �첽���� </returns>
    private async UniTask LoadNewScene(SceneID sceneId)
    {
        var sceneName = sceneId.ToString();
#if UNITY_EDITOR
        // �����׶�EDITORģʽ��,��֤����������
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

        // ��ؼ��ؽ���
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
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName) // ɾ���ļ���չ��
                return true;
        }
        return false;
    }
#endif

    /// <summary>
    /// ���������Դ��Ԥ����(��Ƶ��Ԥ�����)
    /// </summary>
    /// <returns> �첽���� </returns>
    private async UniTask PreloadSceneResources()
    {
        await UniTask.CompletedTask;

        // TODO: Ԥ���س��������Դ
        // 
        // ���´���Ϊʾ��, ֮������
        // 
        // const int totalSteps = 3;
        // var currentStep = 0;
        //
        // //Ԥ������Ƶ
        // currentStep++;
        // await Addressables.LoadAssetsAsync<AudioClip>("scene_audio", null)
        //     .ToUniTask(Progress.Create<float>(p =>
        //         UpdateCompositeProgress(p, currentStep, totalSteps)));
        //
        // //Ԥ����Ԥ����
        // currentStep++;
        // await Addressables.LoadAssetsAsync<GameObject>("scene_prefabs", null)
        //     .ToUniTask(Progress.Create<float>(p =>
        //         UpdateCompositeProgress(p, currentStep, totalSteps)));
        //
        // // ������Դ...
    }

    /// <summary>
    /// ����������Ͻ���
    /// </summary>
    /// <param name="partialProgress"> ���Ȱٷֱ� </param>
    /// <param name="currentStep"> ��ǰ���� </param>
    /// <param name="totalSteps"> </param>
    private void UpdateCompositeProgress(float partialProgress, int currentStep, int totalSteps)
    {
        var stepWeight = 1f / totalSteps;
        var baseProgress = (currentStep - 1) * stepWeight;
        LoadingProgress = baseProgress + partialProgress * stepWeight;
        OnLoadingProgressChanged?.Invoke(LoadingProgress);
    }

    /// <summary>
    /// ��ʼ���³���
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
