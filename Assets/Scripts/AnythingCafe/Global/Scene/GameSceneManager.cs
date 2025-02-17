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
            OnSceneLoadStart?.Invoke(sceneToLoad);

            // TODO: ������ʵ�ֳ����л�����

            await UnloadCurrentScene();
            await LoadNewScene(sceneToLoad);

            // TODO: �رն���

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
    /// ж�ص�ǰ����
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
    /// �����µĳ���
    /// </summary>
    /// <param name="sceneId"> ����ID </param>
    /// <returns> �첽���� </returns>
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
        
        InitializeNewScene();
    }

    /// <summary>
    /// ���������Դ��Ԥ����(��Ƶ��Ԥ�����)
    /// </summary>
    /// <returns> �첽���� </returns>
    private async UniTask PreloadSceneResources()
    {
        // TODO: ������ʵ�ֳ��������Դ��Ԥ����
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ��ʼ���³���
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
