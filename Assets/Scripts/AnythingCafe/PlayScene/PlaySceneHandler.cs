using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlaySceneHandler :
    MonoBehaviourSingleton<PlaySceneHandler>,
    ISceneHandler
{
    [Header("Audio")]
    [SerializeField]
    private MusicSoundMeta _playSceneBGM;

    [Header("Layout UI")]
    [SerializeField]
    private CoffeeUI _coffeeUI;
    [SerializeField]
    private DialogueUI _dialogueUI;
    [SerializeField]
    private DiscoverUI _discoverUI;
    [SerializeField]
    private MachineUI _machineUI;
    [SerializeField]
    private MenuUI _menuUI;
    [SerializeField]
    private SideBtnUI _sideBtnUI;

    public async UniTask OnSceneLoad()
    {
        try
        {
            InitScene();
            ShowScene();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[PlaySceneHandler]Cant call OnSceneLoad() : {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneOnLoadFailed));
        }
    }

    public async UniTask OnSceneUnload()
    {
        try
        {
            SoundManager.Instance.StopAllSounds();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[PlaySceneHandler]Cant call OnSceneUnload() : {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.SceneOnLoadFailed));
        }
    }

    private void InitScene()
    {
        UIManager.Instance.ResetMgr();

        UIManager.Instance.RegisterComponent(0, _coffeeUI);
        UIManager.Instance.RegisterComponent(0, _dialogueUI);
        UIManager.Instance.RegisterComponent(0, _discoverUI);
        UIManager.Instance.RegisterComponent(0, _machineUI);
        UIManager.Instance.RegisterComponent(0, _menuUI);
        UIManager.Instance.RegisterComponent(0, _sideBtnUI);
    }

    private void ShowScene()
    {
        SoundManager.Instance.PlaySound(_playSceneBGM.Type, _playSceneBGM.Clip, _playSceneBGM.Loop, _playSceneBGM.DefaultVolume);
    }
}
