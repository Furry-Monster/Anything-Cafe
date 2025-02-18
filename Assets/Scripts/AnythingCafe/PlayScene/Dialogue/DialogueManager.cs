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
    /// ��ʼ�Ի�, �����ǰ�жԻ����ڽ���, ���Ƚ�����ǰ�Ի��ٿ�ʼ�µĶԻ�
    /// </summary>
    /// <param name="player"> �Ի������� </param>
    /// <returns> �첽���� </returns>
    public async UniTask StartDialogue(DialoguePlayer player)
    {
        if (_currentPlayer != null)
            await StopDialogue();

        _currentPlayer = player;

        await _currentPlayer.StartDialogue();
    }

    /// <summary>
    /// �����Ի�
    /// </summary>
    /// <returns> �첽���� </returns>
    public async UniTask StopDialogue()
    {
        if (_currentPlayer == null) return;

        await _currentPlayer.EndDialogue();

        _currentPlayer = null;
    }
}
