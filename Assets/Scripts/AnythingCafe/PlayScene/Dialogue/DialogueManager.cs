using Cysharp.Threading.Tasks;
using UnityEngine;

[AddComponentMenu("FrameMonster/Dialogue/DialogueManager")]
public class DialogueManager :
    MonoBehaviourSingleton<DialogueManager>,
    IInitializable
{
    private DialoguePlayer _currentPlayer;

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        _currentPlayer = null;
    }

    /// <summary>
    /// 开始对话, 如果当前有对话正在进行, 则先结束当前对话再开始新的对话
    /// </summary>
    /// <param name="player"> 对话播放器 </param>
    /// <returns> 异步任务 </returns>
    public async UniTask StartDialogue(DialoguePlayer player)
    {
        if (_currentPlayer != null)
            await StopDialogue();

        _currentPlayer = player;

        await _currentPlayer.StartDialogue();
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    /// <returns> 异步任务 </returns>
    public async UniTask StopDialogue()
    {
        if (_currentPlayer == null) return;

        await _currentPlayer.EndDialogue();

        _currentPlayer = null;
    }
}
