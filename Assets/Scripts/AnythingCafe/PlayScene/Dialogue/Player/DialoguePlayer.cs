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
    /// �����Ի�������
    /// </summary>
    /// <param name="dialogueNodes"> �Ի��ڵ��б� </param>
    /// <returns> DialoguePlayer </returns>
    public static DialoguePlayer CreatePlayer(List<DialogueNode> dialogueNodes)
    {
        return new DialoguePlayer(dialogueNodes);
    }

    /// <summary>
    /// ���һ���Ի����ŵĻص�
    /// </summary>
    /// <param name="onPlayCallback"> �ص� </param>
    /// <returns> DialoguePlayer </returns>
    public DialoguePlayer OnPlay(Action onPlayCallback)
    {
        OnDialoguePlay += onPlayCallback;
        return this;
    }

    /// <summary>
    /// ���һ���Ի������Ļص�
    /// </summary>
    /// <param name="onEndCallback"> �ص� </param>
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

        _currentNode = _dialogueNodes[0]; // Ĭ�ϴӵ�һ���ڵ㿪ʼ����
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

        // TODO: ʵ�ֶԻ������߼�
    }
}
