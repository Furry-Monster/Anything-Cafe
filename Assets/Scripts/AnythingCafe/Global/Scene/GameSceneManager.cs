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
    /// ���س���
    /// </summary>
    /// <param name="sceneToLoad"> ����ID </param>
    /// <param name="loadingName"></param>
    /// <returns> �첽���� </returns>
    /// <exception cref="CustomErrorException"> ��������ʧ�� </exception>
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
    /// ж�ص�ǰ����
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
                Debug.LogError($"����ж�ش���: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// �����µĳ���
    /// </summary>
    /// <param name="sceneId"></param>
    /// <returns></returns>
    private async UniTask LoadNewScene(SceneID sceneId)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneId.ToString());
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
        await InitializeNewScene();
    }

    /// <summary>
    /// ���������Դ��Ԥ����(��Ƶ��Ԥ�����)
    /// </summary>
    /// <returns></returns>
    private async UniTask PreloadSceneResources()
    {
        // TODO: ������ʵ�ֳ��������Դ��Ԥ����
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ��ʼ���³���
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
        Debug.Log($"�����������: {scene.name} in mode {mode}");


    private static void OnSceneUnloaded(Scene scene) =>
        Debug.Log($"����ж�����: {scene.name}");


    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        base.OnDestroy();
    }
}
