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
    private async UniTask InitializeNewScene()
    {
        var newSceneHandler = GameObject.FindWithTag("SceneHandler").GetComponent<ISceneHandler>();

        if (newSceneHandler != null)
        {
            CurrentSceneHandler = newSceneHandler;
            await CurrentSceneHandler.OnSceneLoad();
        }
    }
}
