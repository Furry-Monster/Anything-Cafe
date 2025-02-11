using System;
using Cysharp.Threading.Tasks;
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
    private DialogUI _dialogUI;
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
}
