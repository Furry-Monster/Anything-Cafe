using System;
using System.Collections.Generic;

public class DialoguePlayer
{
    private readonly List<DialogueNode> _dialogueNodes;
    private DialogueNode _currentNode;

    private event Action OnDialoguePlay;
    private event Action OnDialogueEnd;


    #region Builder
    private DialoguePlayer(List<DialogueNode> dialogueNodes)
    {
        _dialogueNodes = dialogueNodes;
    }

    /// <summary>
    /// 创建对话播放器
    /// </summary>
    /// <param name="dialogueNodes"> 对话节点列表 </param>
    /// <returns> DialoguePlayer </returns>
    public static DialoguePlayer CreatePlayer(List<DialogueNode> dialogueNodes)
    {
        return new DialoguePlayer(dialogueNodes);
    }

    /// <summary>
    /// 添加一个对话播放的回调
    /// </summary>
    /// <param name="onPlayCallback"> 回调 </param>
    /// <returns> DialoguePlayer </returns>
    public DialoguePlayer OnPlay(Action onPlayCallback)
    {
        OnDialoguePlay += onPlayCallback;
        return this;
    }

    /// <summary>
    /// 添加一个对话结束的回调
    /// </summary>
    /// <param name="onEndCallback"> 回调 </param>
    /// <returns> DialoguePlayer </returns>
    public DialoguePlayer OnEnd(Action onEndCallback)
    {
        OnDialogueEnd += onEndCallback;
        return this;
    }
    #endregion

    public void StartDialogue()
    {
        OnDialoguePlay?.Invoke();
        OnDialoguePlay = null;

        _currentNode = _dialogueNodes[0]; // 默认从第一个节点开始播放
        PlayNode(_currentNode);
    }

    public void UpdateDialogue()
    {

    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }

    private void PlayNode(DialogueNode node)
    {
        var data = new DialogueUIModel()
        {
            
        };
        UIManager.Instance.OpenReactive<DialogueUI, DialogueUIModel>(data);

        // TODO: 实现对话播放逻辑
    }
}
