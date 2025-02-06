using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviourSingleton<TitleSceneHandler>, ISceneHandler
{
    [Header("Audio")]
    [SerializeField] private string _titleBGM; // TODO: ��soundManager�洢��Music�л�ȡ���õ�Musicö�٣�����Inspector����ʾ

    [Header("UI Layout")]
    [SerializeField]
    private WarningUI _warningUI;
    [SerializeField]
    private TitleUI _titleUI;
    [SerializeField]
    private ThanksUI _thanksUI;
    [SerializeField]
    private SupportUI _supportUI;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject _backGround;

    public async UniTask OnSceneLoad()
    {
        try
        {
            await InitScene();
            await ShowTitleScript();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[TitleSceneHandler] Scene on load failed. Error message: {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneOnLoadFailed));
        }

    }

    public async UniTask OnSceneUnload()
    {

    }

    private async UniTask InitScene()
    {
        // TODO: ��ʼ������
    }

    private async UniTask ShowTitleScript()
    {
        // ��ʾWarningUI�����ڵ�һ�ν���TitleSceneʱ��ʾ
        if (GameManager.Instance.IsFirstInTitleScene)
        {
#if UNITY_EDITOR
            _warningUI.OnCloseStart += () =>
            {
                _backGround.SetActive(true);
                _titleUI.gameObject.SetActive(true);
                GameManager.Instance.IsFirstInTitleScene = false;
            };

            _backGround.SetActive(false);
            _titleUI.gameObject.SetActive(false);

            await _warningUI.Open();
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await _warningUI.Close();
#endif
        }
        
    }
}
