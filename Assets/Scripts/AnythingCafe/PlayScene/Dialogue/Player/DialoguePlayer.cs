using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

    #region API
    /// <summary>
    /// ��ʼ���ŶԻ�
    /// </summary>
    /// <returns> UniTask </returns>
    public async UniTask StartDialogue()
    {
        OnDialoguePlay?.Invoke();
        OnDialoguePlay = null;

        _currentNode = _dialogueNodes[0]; // Ĭ�ϴӵ�һ���ڵ㿪ʼ����

        var model = new DialogueUIModel()
        {
            Text = "",
            NextBtnData = new ButtonDataTemplate("Next", null, true, UpdateDialogue),
        };
        await UIManager.Instance.OpenReactive<DialogueUI, DialogueUIModel>(model);
    }

    /// <summary>
    /// ���¶Ի�����
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> �ڵ����Ͳ�֧�� </exception>
    public void UpdateDialogue()
    {
        _currentNode.IsPlayed = true;
        _currentNode = _dialogueNodes.Find(node => node.NodeId == _currentNode.NextNodeId);

        switch (_currentNode.NodeAction)
        {
            case NodeAction.Choice:
                // TODO: ����ѡ��ѡ��
                break;
            case NodeAction.Input:
                // TODO: ��������ѡ��
                break;
            case NodeAction.Branch:
                // TODO: �����֧ѡ��
                break;
            case NodeAction.End:
                _ = DialogueManager.Instance.StopDialogue();
                break;
            case NodeAction.None:
            case NodeAction.Chat:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var model = new DialogueUIModel()
        {
            Text = _currentNode.Text,
            NextBtnData = new ButtonDataTemplate("Next", null, true, UpdateDialogue),
        };
        UIManager.Instance.LoadDataReactive<DialogueUI, DialogueUIModel>(model);
    }

    /// <summary>
    /// �����Ի�
    /// </summary>
    /// <returns> UniTask </returns>
    public async UniTask EndDialogue()
    {
        await UIManager.Instance.CloseReactive<DialogueUI>();

        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }
    #endregion
}
