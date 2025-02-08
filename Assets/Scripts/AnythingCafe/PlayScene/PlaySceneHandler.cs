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
        throw new System.NotImplementedException();
    }

    public async UniTask OnSceneUnload()
    {
        throw new System.NotImplementedException();
    }
}
